
namespace FolderSync.Application.Services
{
    /// <summary>
    /// 依模式執行同步（套用／還原）並管理備份紀錄。
    /// </summary>
    public class FolderSynchronizationService
    {
        private readonly IFolderSynchronizerFactory _factory;
        private readonly IBackupRepository _backups;

        public FolderSynchronizationService(IFolderSynchronizerFactory factory, IBackupRepository backups)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _backups = backups ?? throw new ArgumentNullException(nameof(backups));
        }

        public IReadOnlyList<SyncMode> Modes => _factory.Modes;

        public Task ApplyAsync(IReadOnlyList<FileStatus> changes, string sourceDir, string destDir, SyncMode mode)
        {
            return _factory.Create(mode).ApplyAsync(changes, sourceDir, destDir);
        }

        public Task RollbackAsync(string backupDir, string destDir, SyncMode mode)
        {
            return _factory.Create(mode).RollbackAsync(backupDir, destDir);
        }

        public IReadOnlyList<BackupFolder> ListBackups(string sourceDir, string destDir)
        {
            return _backups.ListBackups(sourceDir, destDir);
        }

        public void DeleteBackup(string fullPath)
        {
            _backups.Delete(fullPath);
        }
    }
}
