namespace FolderSyncCore.Imps
{
    internal class AsyncFolderControl : IAsyncFolderControl
    {
        private readonly IFolderReader _reader;

        private string DeleteName => FolderControl.DeleteName;
        private string DiffName => FolderControl.DiffName;
        private string AddName => FolderControl.AddName;

        public AsyncFolderControl(IFolderReader reader)
        {
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        public async Task OverwriteAsync(IEnumerable<FileStatus> files, string sourceDir, string destDir)
        {
            var backupDir = _reader.CreateBackupDirectory(sourceDir, destDir);
            var backupTasks = new List<Task>() {
                BackupAsync(files, backupDir, Path.GetFileName(sourceDir), x => x.來源路徑, CompareState.新增檔案, CompareState.時間不同),
                BackupAsync(files, backupDir, Path.GetFileName(destDir), x => x.目標路徑, CompareState.刪除檔案, CompareState.時間不同),
                BackupAsync(files, backupDir, DeleteName, x => x.目標路徑, CompareState.刪除檔案),
                BackupAsync(files, backupDir, DiffName, x => x.目標路徑, CompareState.時間不同),
                BackupAsync(files, backupDir, AddName, x => x.來源路徑, CompareState.新增檔案),
            };
            await Task.WhenAll(backupTasks);
            await OverwriteAsync(files, destDir);
        }

        private Task BackupAsync(IEnumerable<FileStatus> files, string host, string name, Func<FileStatus, string> func, params CompareState[] states)
        {
            var filters = files
                .Where(x => states.Contains(x.狀態))
                .ToList();
            var backupDir = Path.Combine(host, name);
            return CopyAsync(filters, backupDir, func);
        }

        internal virtual async Task CopyAsync(IEnumerable<FileStatus> files, string destHost, Func<FileStatus, string> func)
        {
            var copyTasks = files.Select(file => Task.Run(() =>
            {
                var sourcePath = func.Invoke(file);
                var destPath = Path.Combine(destHost, file.相對路徑);
                var destDir = Path.GetDirectoryName(destPath);
                CreateDirectory(destDir);
                File.Copy(sourcePath, destPath, true);
            }));

            await Task.WhenAll(copyTasks);
        }

        private Task OverwriteAsync(IEnumerable<FileStatus> files, string destDir)
        {
            var copyFiles = files
                .Where(x => x.狀態 == CompareState.新增檔案 || x.狀態 == CompareState.時間不同)
                .ToList();
            var copyTask = CopyAsync(copyFiles, destDir, x => x.來源路徑);

            var deleteFiles = files.Where(x => x.狀態 == CompareState.刪除檔案).ToList();
            var deleteTask = DeleteAsync(deleteFiles, x => x.目標路徑);

            return Task.WhenAll(copyTask, deleteTask);
        }


        private static void CreateDirectory(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        internal virtual Task DeleteAsync(IEnumerable<FileStatus> dtos, Func<FileStatus, string> func)
        {
            var deleteTasks = dtos.Select(dto => Task.Run(() =>
            {
                var path = func.Invoke(dto);
                File.Delete(path);
            }));
            return Task.WhenAll(deleteTasks);
        }

        public Task RestoreAsync(string backupDir, string destDir)
        {
            //反向操作
            var deleteFiles = _reader.GetBackupFiles(backupDir, DeleteName);
            var restoreDeleteTask = CopyAsync(deleteFiles, destDir, x => x.來源路徑);

            var diffFiles = _reader.GetBackupFiles(backupDir, DiffName);
            var restoreCopyTask = CopyAsync(diffFiles, destDir, x => x.來源路徑);

            var addFiles = _reader.GetBackupFiles(backupDir, AddName)
                .Select(x => new FileStatus(x.相對路徑, Path.Combine(destDir, x.相對路徑), null))
                .ToList();
            var restoreAddTask = DeleteAsync(addFiles, x => x.來源路徑);
            return Task.WhenAll(restoreDeleteTask, restoreCopyTask, restoreAddTask);
        }
    }
}
