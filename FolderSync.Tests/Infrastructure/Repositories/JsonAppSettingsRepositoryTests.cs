namespace FolderSync.Tests.Infrastructure.Repositories
{
    public class JsonAppSettingsRepositoryTests : IDisposable
    {
        private readonly string _path;

        public JsonAppSettingsRepositoryTests()
        {
            _path = Path.Combine(Path.GetTempPath(), "FolderSyncSettings_" + Guid.NewGuid().ToString("N") + ".json");
        }

        [Fact]
        public void SaveThenLoad_往返保留設定()
        {
            // Arrange
            var sut = new JsonAppSettingsRepository(_path);
            var settings = new AppSettings
            {
                Source = "C:\\src",
                Dest = "C:\\dst",
                IgnoreFiles = new[] { "a.txt" },
                IgnoreFolders = new[] { "logs" },
                SiteDelay = 1234,
            };

            // Act
            sut.Save(settings);
            var loaded = sut.Load();

            // Assert
            Assert.Equal("C:\\src", loaded.Source);
            Assert.Equal("C:\\dst", loaded.Dest);
            Assert.Equal(new[] { "a.txt" }, loaded.IgnoreFiles);
            Assert.Equal(new[] { "logs" }, loaded.IgnoreFolders);
            Assert.Equal(1234, loaded.SiteDelay);
        }

        [Fact]
        public void Load_檔案不存在_回傳預設設定()
        {
            // Arrange
            var sut = new JsonAppSettingsRepository(_path);

            // Act
            var loaded = sut.Load();

            // Assert
            Assert.Equal("", loaded.Source);
        }

        [Fact]
        public void Load_內容毀損_回傳預設設定()
        {
            // Arrange
            File.WriteAllText(_path, "{ not valid json");
            var sut = new JsonAppSettingsRepository(_path);

            // Act
            var loaded = sut.Load();

            // Assert
            Assert.Equal("", loaded.Source);
        }

        public void Dispose()
        {
            if (File.Exists(_path))
            {
                File.Delete(_path);
            }
        }
    }
}
