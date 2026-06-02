
namespace FolderSync.Application.Services
{
    public class FolderSyncService
    {
        private readonly FolderDiffService _diff;
        private readonly FolderSynchronizationService _sync;

        public FolderSyncService(FolderDiffService diff, FolderSynchronizationService sync)
        {
            _diff = diff ?? throw new ArgumentNullException(nameof(diff));
            _sync = sync ?? throw new ArgumentNullException(nameof(sync));
        }

        public IReadOnlyList<SyncMode> GetSyncModes()
        {
            return _sync.Modes;
        }

        public List<FileStatus> GetDiffFiles(string sourceDir, string destDir)
        {
            return _diff.GetDiff(sourceDir, destDir);
        }

        public List<BackupFolder> GetBackupFolders(string sourceDir, string destDir)
        {
            return _sync.ListBackups(sourceDir, destDir).ToList();
        }

        public Task ApplyAsync(string sourceDir, string destDir, SyncMode mode)
        {
            var changes = _diff.GetDiff(sourceDir, destDir);
            return _sync.ApplyAsync(changes, sourceDir, destDir, mode);
        }

        public Task RollbackAsync(IReadOnlyList<BackupFolder> selected, string destDir, SyncMode mode)
        {
            var backupDir = RequireSingleBackupPath(selected);
            return _sync.RollbackAsync(backupDir, destDir, mode);
        }

        public void DeleteBackups(IReadOnlyList<BackupFolder> selected)
        {
            if (selected.Count == 0)
            {
                throw new DomainException("選擇的備份紀錄不存在");
            }

            foreach (var folder in selected)
            {
                _sync.DeleteBackup(folder.完整路徑);
            }
        }

        private static string RequireSingleBackupPath(IReadOnlyList<BackupFolder> selected)
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
