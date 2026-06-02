using FolderSync.Domain;

namespace FolderSync.Application
{
    public class FolderSyncService
    {
        private readonly IFolderReader _reader;
        private readonly IFolderControlFactory _factory;

        public FolderSyncService(IFolderReader reader, IFolderControlFactory factory)
        {
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public string[] GetNames()
        {
            return _factory.GetNames();
        }

        public List<FileStatus> GetDiffFiles(string sourceDir, string destDir)
        {
            return _reader.GetDiffFiles(sourceDir, destDir);
        }

        public List<BackupFolder> GetBackupFolders(string sourceDir, string destDir)
        {
            return _reader.GetBackupFolders(sourceDir, destDir);
        }

        public void Overwrite(string sourceDir, string destDir, string type)
        {
            var files = _reader.GetDiffFiles(sourceDir, destDir);
            _factory.CreateControl(type).Overwrite(files, sourceDir, destDir);
        }

        public Task OverwriteAsync(string sourceDir, string destDir, string type)
        {
            var files = _reader.GetDiffFiles(sourceDir, destDir);
            return _factory.CreateAsyncControl(type).OverwriteAsync(files, sourceDir, destDir);
        }

        public void Restore(IReadOnlyList<BackupFolder> selected, string destDir, string type)
        {
            var backupDir = SingleBackupPath(selected);
            _factory.CreateControl(type).Restore(backupDir, destDir);
        }

        public Task RestoreAsync(IReadOnlyList<BackupFolder> selected, string destDir, string type)
        {
            var backupDir = SingleBackupPath(selected);
            return _factory.CreateAsyncControl(type).RestoreAsync(backupDir, destDir);
        }

        private static string SingleBackupPath(IReadOnlyList<BackupFolder> selected)
        {
            if (selected.Count == 0)
            {
                throw new Exception("選擇的備份紀錄不存在");
            }

            if (selected.Count > 1)
            {
                throw new Exception("要還原的檔案只能選一個");
            }

            return selected[0].完整路徑;
        }

        public void DeleteBackups(IReadOnlyList<BackupFolder> selected)
        {
            if (selected.Count == 0)
            {
                throw new Exception("選擇的備份紀錄不存在");
            }

            foreach (var folder in selected)
            {
                _reader.DeleteBackupFolder(folder.完整路徑);
            }
        }
    }
}
