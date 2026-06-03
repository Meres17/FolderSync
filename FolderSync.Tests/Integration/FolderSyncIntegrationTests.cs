
namespace FolderSync.Tests.Integration
{
    /// <summary>
    /// 以真實檔案系統驗證掃描、比對、套用與還原的端到端行為。
    /// 同一類別的測試循序執行，避免共用目前工作目錄造成干擾。
    /// </summary>
    [Collection("FileSystem")]
    public class FolderSyncIntegrationTests : IDisposable
    {
        private static readonly DateTime OldTime = new(2023, 1, 1, 9, 0, 0);
        private static readonly DateTime NewTime = new(2023, 6, 1, 9, 0, 0);

        private readonly string _root;
        private readonly string _originalCurrentDir;
        private readonly string _source;
        private readonly string _dest;

        public FolderSyncIntegrationTests()
        {
            _originalCurrentDir = Directory.GetCurrentDirectory();
            _root = Path.Combine(Path.GetTempPath(), "FolderSyncTest_" + Guid.NewGuid().ToString("N"));
            _source = Path.Combine(_root, "src");
            _dest = Path.Combine(_root, "dst");
            Directory.CreateDirectory(_source);
            Directory.CreateDirectory(_dest);
            // 備份會建立在目前工作目錄下，切換到測試專用根目錄以隔離
            Directory.SetCurrentDirectory(_root);
        }

        [Fact]
        public void Compare_回傳新增刪除與時間不同的檔案()
        {
            // Arrange
            SeedScenario();
            var scanner = new FolderScanner();

            // Act
            var diff = new FolderComparer().Compare(
                scanner.Scan(_source), scanner.Scan(_dest), IgnorePolicy.From(new AppSettings()));

            // Assert
            Assert.Equal(3, diff.Count);
            Assert.Contains(diff, x => x.檔名 == "a.txt" && x.狀態 == CompareState.時間不同);
            Assert.Contains(diff, x => x.檔名 == "b.txt" && x.狀態 == CompareState.新增檔案);
            Assert.Contains(diff, x => x.檔名 == "c.txt" && x.狀態 == CompareState.刪除檔案);
        }

        [Fact]
        public async Task ApplyThenRollback_目標資料夾還原到更新前()
        {
            // Arrange
            SeedScenario();
            var scanner = new FolderScanner();
            var settings = new AppSettingsProvider(new JsonAppSettingsRepository(Path.Combine(_root, "appsettings.json")));
            var backups = new FileSystemBackupRepository(scanner, settings, TimeProvider.System);
            var synchronizer = new FolderSynchronizerFactory(backups, settings, new FileOperations(), new SiteController()).Create(SyncMode.資料夾);
            var diff = new FolderComparer().Compare(
                scanner.Scan(_source), scanner.Scan(_dest), IgnorePolicy.From(new AppSettings()));

            // Act：套用
            await synchronizer.ApplyAsync(diff, _source, _dest);

            // Assert：目標已套用來源差異
            Assert.Equal("SRC-A", File.ReadAllText(Path.Combine(_dest, "a.txt")));
            Assert.True(File.Exists(Path.Combine(_dest, "b.txt")));
            Assert.False(File.Exists(Path.Combine(_dest, "c.txt")));

            // Act：還原
            var backup = backups.ListBackups(_source, _dest).Single();
            await synchronizer.RollbackAsync(backup.完整路徑, _dest);

            // Assert：目標回到更新前
            Assert.Equal("DST-A", File.ReadAllText(Path.Combine(_dest, "a.txt")));
            Assert.False(File.Exists(Path.Combine(_dest, "b.txt")));
            Assert.Equal("DST-C", File.ReadAllText(Path.Combine(_dest, "c.txt")));
        }

        /// <summary>
        /// 佈置情境：a.txt 時間不同、b.txt 僅來源（新增）、c.txt 僅目標（刪除）、same.txt 時間相同。
        /// </summary>
        private void SeedScenario()
        {
            WriteFile(Path.Combine(_source, "a.txt"), "SRC-A", NewTime);
            WriteFile(Path.Combine(_dest, "a.txt"), "DST-A", OldTime);

            WriteFile(Path.Combine(_source, "b.txt"), "SRC-B", NewTime);

            WriteFile(Path.Combine(_dest, "c.txt"), "DST-C", OldTime);

            WriteFile(Path.Combine(_source, "same.txt"), "SAME", OldTime);
            WriteFile(Path.Combine(_dest, "same.txt"), "SAME", OldTime);
        }

        private static void WriteFile(string path, string content, DateTime time)
        {
            File.WriteAllText(path, content);
            File.SetLastWriteTime(path, time);
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
