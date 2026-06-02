using NSubstitute;

namespace FolderSync.Tests.Infrastructure.Repositories
{
    /// <summary>
    /// 備份倉儲會以目前工作目錄為根，故切換到測試專用根目錄以隔離；同類別測試循序執行。
    /// </summary>
    [Collection("FileSystem")]
    public class FileSystemBackupRepositoryTests : IDisposable
    {
        private readonly string _root;
        private readonly string _originalCurrentDir;
        private readonly string _source;
        private readonly string _dest;

        public FileSystemBackupRepositoryTests()
        {
            _originalCurrentDir = Directory.GetCurrentDirectory();
            _root = Path.Combine(Path.GetTempPath(), "FolderSyncBackup_" + Guid.NewGuid().ToString("N"));
            _source = Path.Combine(_root, "src");
            _dest = Path.Combine(_root, "dst");
            Directory.CreateDirectory(_root);
            Directory.SetCurrentDirectory(_root);
        }

        [Fact]
        public void ListBackups_只回傳時間戳命名的資料夾()
        {
            // Arrange
            var sut = NewRepository();
            var backupRoot = Path.Combine(_root, "Backup", "src_dst");
            Directory.CreateDirectory(Path.Combine(backupRoot, "20230101_1200"));
            Directory.CreateDirectory(Path.Combine(backupRoot, "not-a-timestamp"));

            // Act
            var backups = sut.ListBackups(_source, _dest);

            // Assert
            Assert.Single(backups);
            Assert.Equal("20230101_1200", backups[0].備份名稱);
        }

        [Fact]
        public void ReadBackup_分類資料夾不存在_回傳空清單()
        {
            // Arrange
            var sut = NewRepository();

            // Act
            var result = sut.ReadBackup(Path.Combine(_root, "nonexistent"), BackupCategory.Delete);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void CreateBackupDirectory_位於來源_目標的備份根目錄下()
        {
            // Arrange
            var sut = NewRepository();

            // Act
            var dir = sut.CreateBackupDirectory(_source, _dest);

            // Assert
            Assert.StartsWith(Path.Combine(_root, "Backup", "src_dst"), dir);
        }

        [Fact]
        public void CreateBackupDirectory_超過保留數_清理最舊備份()
        {
            // Arrange
            var sut = NewRepository(retention: 3);
            var backupRoot = Path.Combine(_root, "Backup", "src_dst");
            foreach (var name in new[] { "20230101_1200", "20230102_1200", "20230103_1200", "20230104_1200", "20230105_1200" })
            {
                Directory.CreateDirectory(Path.Combine(backupRoot, name));
            }

            // Act
            sut.CreateBackupDirectory(_source, _dest);

            // Assert：保留最新 retention-1 = 2 個（加上即將建立的新備份共 3）
            var remaining = sut.ListBackups(_source, _dest).Select(x => x.備份名稱).OrderBy(x => x).ToList();
            Assert.Equal(new[] { "20230104_1200", "20230105_1200" }, remaining);
        }

        private static FileSystemBackupRepository NewRepository(int retention = 10)
        {
            var repo = Substitute.For<IAppSettingsRepository>();
            repo.Load().Returns(new AppSettings { RetentionCount = retention });
            return new FileSystemBackupRepository(new FolderScanner(), new AppSettingsProvider(repo));
        }

        public void Dispose()
        {
            Directory.SetCurrentDirectory(_originalCurrentDir);
            try
            {
                Directory.Delete(_root, true);
            }
            catch
            {
                // 清理失敗不影響測試結果
            }
        }
    }
}
