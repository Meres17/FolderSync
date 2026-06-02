using System.Globalization;


namespace FolderSync.Infrastructure.Repositories
{
    public class FileSystemBackupRepository : IBackupRepository
    {
        private const string Format = "yyyyMMdd_HHmm";
        private readonly IFolderScanner _scanner;

        public FileSystemBackupRepository(IFolderScanner scanner)
        {
            _scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));
        }

        public string CreateBackupDirectory(string sourceDir, string destDir)
        {
            var host = BuildBackupHost(sourceDir, destDir);
            var time = DateTime.Now.ToString(Format);
            return Path.Combine(host, time);
        }

        public IReadOnlyList<BackupFolder> ListBackups(string sourceDir, string destDir)
        {
            var host = BuildBackupHost(sourceDir, destDir);
            return Directory
                .EnumerateDirectories(host, "*", SearchOption.TopDirectoryOnly)
                .Where(x => DateTime.TryParseExact(Path.GetFileName(x), Format, null, DateTimeStyles.None, out _))
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

        public void Delete(string fullPath)
        {
            Directory.Delete(fullPath, true);
        }

        private static string BuildBackupHost(string sourceDir, string destDir)
        {
            var host = Directory.GetCurrentDirectory();
            var sourceName = Path.GetFileName(sourceDir);
            var destName = Path.GetFileName(destDir);
            var result = Path.Combine(host, "Backup", $"{sourceName}_{destName}");
            if (!Directory.Exists(result))
            {
                Directory.CreateDirectory(result);
            }
            return result;
        }
    }
}
