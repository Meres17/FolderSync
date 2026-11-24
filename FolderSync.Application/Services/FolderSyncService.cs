
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
        /// 有差異但符合忽略規則、更新時不會被覆蓋的檔案。
        /// </summary>
        public List<FileStatus> GetIgnoredDiffFiles(string sourceDir, string destDir)
        {
            return _diff.GetIgnoredDiff(RequireExisting(sourceDir, Source), RequireExisting(destDir, Dest));
        }

        /// <summary>
        /// 來源只用來對應備份資料夾名稱，可以已被刪除；目標必須存在。
        /// </summary>
        public List<BackupFolder> GetBackupFolders(string sourceDir, string destDir)
        {
            return _sync.ListBackups(RequireEntered(sourceDir, Source), RequireExisting(destDir, Dest)).ToList();
        }

        public Task ApplyAsync(string sourceDir, string destDir, SyncMode mode)
        {
            var source = RequireExisting(sourceDir, Source);
            var dest = RequireExisting(destDir, Dest);
            var changes = _diff.GetDiff(source, dest);
            return _sync.ApplyAsync(changes, source, dest, mode);
        }

        public Task RollbackAsync(IReadOnlyList<BackupFolder> selected, string destDir, SyncMode mode)
        {
            var backupDir = RequireSingleBackupPath(selected);
            RequireBackupExists(backupDir);
            return _sync.RollbackAsync(backupDir, RequireExisting(destDir, Dest), mode);
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
