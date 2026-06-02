namespace FolderSync.Application.Services
{
    /// <summary>
    /// 依模式執行同步（套用／還原）並管理備份紀錄，並記錄操作稽核。
    /// </summary>
    public class FolderSynchronizationService
    {
        private readonly IFolderSynchronizerFactory _factory;
        private readonly IBackupRepository _backups;
        private readonly IActivityLog _log;

        public FolderSynchronizationService(IFolderSynchronizerFactory factory, IBackupRepository backups, IActivityLog log)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _backups = backups ?? throw new ArgumentNullException(nameof(backups));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public IReadOnlyList<SyncMode> Modes => _factory.Modes;

        public async Task ApplyAsync(IReadOnlyList<FileStatus> changes, string sourceDir, string destDir, SyncMode mode)
        {
            _log.Info($"更新開始：{sourceDir} → {destDir}（{mode}，{changes.Count} 個檔案）");
            try
            {
                await _factory.Create(mode).ApplyAsync(changes, sourceDir, destDir);
                _log.Info($"更新完成：{destDir}");
            }
            catch (Exception ex)
            {
                _log.Error($"更新失敗：{destDir}", ex);
                throw;
            }
        }

        public async Task RollbackAsync(string backupDir, string destDir, SyncMode mode)
        {
            _log.Info($"還原開始：{backupDir} → {destDir}（{mode}）");
            try
            {
                await _factory.Create(mode).RollbackAsync(backupDir, destDir);
                _log.Info($"還原完成：{destDir}");
            }
            catch (Exception ex)
            {
                _log.Error($"還原失敗：{destDir}", ex);
                throw;
            }
        }

        public IReadOnlyList<BackupFolder> ListBackups(string sourceDir, string destDir)
        {
            return _backups.ListBackups(sourceDir, destDir);
        }

        public void DeleteBackup(string fullPath)
        {
            _backups.DeleteBackup(fullPath);
            _log.Info($"刪除備份：{fullPath}");
        }
    }
}
