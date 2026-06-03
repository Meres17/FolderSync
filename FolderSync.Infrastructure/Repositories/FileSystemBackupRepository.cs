using System.Globalization;

namespace FolderSync.Infrastructure.Repositories
{
    public class FileSystemBackupRepository : IBackupRepository
    {
        private const string Format = "yyyyMMdd_HHmmss";
        private readonly IFolderScanner _scanner;
        private readonly AppSettingsProvider _settings;
        private readonly TimeProvider _clock;

        public FileSystemBackupRepository(IFolderScanner scanner, AppSettingsProvider settings, TimeProvider clock)
        {
            _scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        }

        public string CreateBackupDirectory(string sourceDir, string destDir)
        {
            var root = GetBackupRoot(sourceDir, destDir);
            PruneOldBackups(root);
            return Path.Combine(root, _clock.GetLocalNow().ToString(Format));
        }

        public IReadOnlyList<BackupFolder> ListBackups(string sourceDir, string destDir)
        {
            var root = GetBackupRoot(sourceDir, destDir);
            return EnumerateBackupDirs(root)
                .Select(x => new BackupFolder(x))
                .ToList();
        }

        public IReadOnlyList<FileSnapshot> ReadBackup(string backupDir, string category)
        {
            var dir = Path.Combine(backupDir, category);
            if (!Directory.Exists(dir)) //還原時，未必會有新/刪/修資料夾
            {
                return Array.Empty<FileSnapshot>();
            }
            return _scanner.Scan(dir);
        }

        public void DeleteBackup(string fullPath)
        {
            Directory.Delete(fullPath, true);
        }

        /// <summary>
        /// 依保留設定清理舊備份，保留最新的 (RetentionCount - 1) 個，
        /// 連同即將建立的新備份共 RetentionCount 個；設為 0 表示不限制。
        /// </summary>
        private void PruneOldBackups(string root)
        {
            var keep = _settings.Current.RetentionCount;
            if (keep <= 0)
            {
                return;
            }

            var stale = EnumerateBackupDirs(root)
                .OrderByDescending(Path.GetFileName)
                .Skip(keep - 1)
                .ToList();

            foreach (var dir in stale)
            {
                Directory.Delete(dir, true);
            }
        }

        private static IEnumerable<string> EnumerateBackupDirs(string root)
        {
            return Directory
                .EnumerateDirectories(root, "*", SearchOption.TopDirectoryOnly)
                .Where(x => DateTime.TryParseExact(Path.GetFileName(x), Format, null, DateTimeStyles.None, out _));
        }

        private static string GetBackupRoot(string sourceDir, string destDir)
        {
            var current = Directory.GetCurrentDirectory();
            var sourceName = Path.GetFileName(sourceDir);
            var destName = Path.GetFileName(destDir);
            var result = Path.Combine(current, "Backup", $"{sourceName}_{destName}");
            if (!Directory.Exists(result))
            {
                Directory.CreateDirectory(result);
            }
            return result;
        }
    }
}
