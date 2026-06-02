using FolderSync.Domain;

namespace FolderSync.Application
{
    public class FolderSyncService
    {
        private readonly IFolderScanner _scanner;
        private readonly FolderComparer _comparer;
        private readonly IgnorePolicy _ignore;
        private readonly IBackupRepository _backups;
        private readonly IFolderSynchronizerFactory _factory;

        public FolderSyncService(
            IFolderScanner scanner,
            FolderComparer comparer,
            IgnorePolicy ignore,
            IBackupRepository backups,
            IFolderSynchronizerFactory factory)
        {
            _scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));
            _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            _ignore = ignore ?? throw new ArgumentNullException(nameof(ignore));
            _backups = backups ?? throw new ArgumentNullException(nameof(backups));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public IReadOnlyList<SyncMode> GetSyncModes()
        {
            return _factory.Modes;
        }

        public List<FileStatus> GetDiffFiles(string sourceDir, string destDir)
        {
            var source = _scanner.Scan(sourceDir);
            var dest = _scanner.Scan(destDir);
            return _comparer.Compare(source, dest, _ignore);
        }

        public List<BackupFolder> GetBackupFolders(string sourceDir, string destDir)
        {
            return _backups.ListBackups(sourceDir, destDir).ToList();
        }

        public Task ApplyAsync(string sourceDir, string destDir, SyncMode mode)
        {
            var changes = GetDiffFiles(sourceDir, destDir);
            return _factory.Create(mode).ApplyAsync(changes, sourceDir, destDir);
        }

        public Task RollbackAsync(IReadOnlyList<BackupFolder> selected, string destDir, SyncMode mode)
        {
            var backupDir = SingleBackupPath(selected);
            return _factory.Create(mode).RollbackAsync(backupDir, destDir);
        }

        public void DeleteBackups(IReadOnlyList<BackupFolder> selected)
        {
            if (selected.Count == 0)
            {
                throw new DomainException("選擇的備份紀錄不存在");
            }

            foreach (var folder in selected)
            {
                _backups.Delete(folder.完整路徑);
            }
        }

        private static string SingleBackupPath(IReadOnlyList<BackupFolder> selected)
        {
            if (selected.Count == 0)
            {
                throw new DomainException("選擇的備份紀錄不存在");
            }

            if (selected.Count > 1)
            {
                throw new DomainException("要還原的檔案只能選一個");
            }

            return selected[0].完整路徑;
        }
    }
}
