
namespace FolderSync.Application.Services
{
    public class FolderSyncService
    {
        private const string Source = "來源";
        private const string Dest = "目標";

        private readonly FolderDiffService _diff;
        private readonly FolderSynchronizationService _sync;
        private readonly IDirectoryProbe _directories;

        public FolderSyncService(FolderDiffService diff, FolderSynchronizationService sync, IDirectoryProbe directories)
        {
            _diff = diff ?? throw new ArgumentNullException(nameof(diff));
            _sync = sync ?? throw new ArgumentNullException(nameof(sync));
            _directories = directories ?? throw new ArgumentNullException(nameof(directories));
        }

        public IReadOnlyList<SyncMode> GetSyncModes()
        {
            return _sync.Modes;
        }

        public List<FileStatus> GetDiffFiles(string sourceDir, string destDir)
        {
            return _diff.GetDiff(RequireExisting(sourceDir, Source), RequireExisting(destDir, Dest));
        }

        /// <summary>
        /// 掃描一次產生更新計畫（含略過清單）供確認；沒有變更時直接擋下，不必進確認視窗。
        /// </summary>
        public UpdatePlan PrepareUpdate(string sourceDir, string destDir)
        {
            var plan = _diff.GetPlan(RequireExisting(sourceDir, Source), RequireExisting(destDir, Dest));
            if (plan.變更.Count == 0)
            {
                throw new DomainException("沒有需要更新的檔案");
            }

            return plan;
        }

        /// <summary>
        /// 來源只用來對應備份資料夾名稱，可以已被刪除；目標必須存在。
        /// </summary>
        public List<BackupFolder> GetBackupFolders(string sourceDir, string destDir)
        {
            return _sync.ListBackups(RequireEntered(sourceDir, Source), RequireExisting(destDir, Dest)).ToList();
        }

        /// <summary>
        /// 依確認過的計畫套用，不再重新掃描；確認期間資料夾可能被刪，故再檢查一次存在。
        /// </summary>
        public Task ApplyAsync(UpdatePlan plan, SyncMode mode, IProgress<SyncProgress>? progress = null)
        {
            var source = RequireExisting(plan.來源, Source);
            var dest = RequireExisting(plan.目標, Dest);
            return _sync.ApplyAsync(plan.變更, source, dest, mode, progress);
        }

        public Task RollbackAsync(IReadOnlyList<BackupFolder> selected, string destDir, SyncMode mode, IProgress<SyncProgress>? progress = null)
        {
            var backupDir = RequireSingleBackupPath(selected);
            RequireBackupExists(backupDir);
            return _sync.RollbackAsync(backupDir, RequireExisting(destDir, Dest), mode, progress);
        }

        public void DeleteBackups(IReadOnlyList<BackupFolder> selected)
        {
            if (selected.Count == 0)
            {
                throw new DomainException("選擇的備份紀錄不存在");
            }

            foreach (var folder in selected)
            {
                RequireBackupExists(folder.完整路徑);
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

        // 去掉結尾的 \，否則 Path.GetFileName 會取到空字串，備份資料夾名稱對不上。
        private static string RequireEntered(string path, string label)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new DomainException($"請選擇{label}資料夾");
            }

            return Path.TrimEndingDirectorySeparator(path.Trim());
        }

        // 每次執行都重新檢查，涵蓋選好路徑後資料夾才被刪除的情況。
        private string RequireExisting(string path, string label)
        {
            var result = RequireEntered(path, label);
            if (_directories.Exists(result) == false)
            {
                throw new DomainException($"找不到{label}資料夾：{result}");
            }

            return result;
        }

        private void RequireBackupExists(string backupDir)
        {
            if (_directories.Exists(backupDir) == false)
            {
                throw new DomainException($"備份紀錄已不存在：{Path.GetFileName(backupDir)}，請重新按「備份紀錄」");
            }
        }
    }
}
