namespace FolderSyncCore.Imps
{
    internal class FolderControl : IFolderControl
    {
        private const string Format = "yyyyMMdd_HHmm";
        internal const string DeleteName = "_state/Delete";
        internal const string DiffName = "_state/Diff";
        internal const string AddName = "_state/Add";
        private readonly IFolderReader _reader;

        public FolderControl(IFolderReader reader)
        {
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        public void Overwrite(IEnumerable<FileStatus> files, string sourceDir, string destDir)
        {
            var backupDir = _reader.CreateBackupDirectory(sourceDir, destDir);
            Backup(files, backupDir, Path.GetFileName(sourceDir), x => x.來源路徑, CompareState.新增檔案, CompareState.時間不同);
            Backup(files, backupDir, Path.GetFileName(destDir), x => x.目標路徑, CompareState.刪除檔案, CompareState.時間不同);
            Backup(files, backupDir, DeleteName, x => x.目標路徑, CompareState.刪除檔案);
            Backup(files, backupDir, DiffName, x => x.目標路徑, CompareState.時間不同);
            Backup(files, backupDir, AddName, x => x.來源路徑, CompareState.新增檔案);

            Overwrite(files, destDir);
        }

        private void Backup(IEnumerable<FileStatus> files, string host, string name, Func<FileStatus, string> func, params CompareState[] states)
        {
            var filters = files
                .Where(x => states.Contains(x.狀態))
                .ToList();
            var backupDir = Path.Combine(host, name);
            Copy(filters, backupDir, func);
        }


        private void Overwrite(IEnumerable<FileStatus> files, string destDir)
        {
            var copyFiles = files
                .Where(x => x.狀態 == CompareState.新增檔案 || x.狀態 == CompareState.時間不同)
                .ToList();
            Copy(copyFiles, destDir, x => x.來源路徑);

            var deleteFiles = files.Where(x => x.狀態 == CompareState.刪除檔案).ToList();
            Delete(deleteFiles, x => x.目標路徑);
        }

        internal virtual void Copy(IEnumerable<FileStatus> files, string destHost, Func<FileStatus, string> func)
        {
            foreach (var file in files)
            {
                var sourcePath = func.Invoke(file);
                var destPath = Path.Combine(destHost, file.相對路徑);
                var destDir = Path.GetDirectoryName(destPath);
                CreateDirectory(destDir);
                File.Copy(sourcePath, destPath, true);
            }
        }

        private static void CreateDirectory(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        internal virtual void Delete(IEnumerable<FileStatus> dtos, Func<FileStatus, string> func)
        {
            foreach (var dto in dtos)
            {
                var path = func.Invoke(dto);
                File.Delete(path);
            }
        }

        public void Restore(string backupDir, string destDir)
        {
            //反向操作
            var deleteFiles = _reader.GetBackupFiles(backupDir, DeleteName);
            Copy(deleteFiles, destDir, x => x.來源路徑);

            var diffFiles = _reader.GetBackupFiles(backupDir, DiffName);
            Copy(diffFiles, destDir, x => x.來源路徑);

            var addFiles = _reader.GetBackupFiles(backupDir, AddName)
                .Select(x => new FileStatus(x.相對路徑, Path.Combine(destDir, x.相對路徑), null))
                .ToList();
            Delete(addFiles, x => x.來源路徑);
        }

    }
}
