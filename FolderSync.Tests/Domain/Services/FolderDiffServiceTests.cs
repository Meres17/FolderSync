
using NSubstitute;

namespace FolderSync.Tests.Domain.Services
{
    public class FolderDiffServiceTests
    {
        [Fact]
        public void GetDiff_掃描來源與目標後比對產生差異()
        {
            // Arrange
            var scanner = Substitute.For<IFolderScanner>();
            scanner.Scan("src").Returns(new[] { new FileSnapshot("a.txt", "full\\a.txt", new DateTime(2023, 1, 1)) });
            scanner.Scan("dst").Returns(Array.Empty<FileSnapshot>());
            var repo = Substitute.For<IAppSettingsRepository>();
            repo.Load().Returns(new AppSettings());
            var sut = new FolderDiffService(scanner, new FolderComparer(), new AppSettingsProvider(repo));

            // Act
            var diff = sut.GetDiff("src", "dst");

            // Assert
            Assert.Single(diff);
            Assert.Equal(CompareState.新增檔案, diff[0].狀態);
        }

        [Fact]
        public void GetIgnoredDiff_依設定的忽略規則_回傳不會被同步的差異檔案()
        {
            // arrange
            var stub = Substitute.For<IFolderScanner>();
            stub.Scan("src").Returns(new[]
            {
                new FileSnapshot("appsettings.Production.json", "full\\appsettings.Production.json", new DateTime(2023, 1, 1)),
                new FileSnapshot("a.txt", "full\\a.txt", new DateTime(2023, 1, 1)),
            });
            stub.Scan("dst").Returns(Array.Empty<FileSnapshot>());
            var sut = new FolderDiffService(stub, new FolderComparer(), CreateSettings(new AppSettings()));

            // act
            var result = sut.GetIgnoredDiff("src", "dst");

            // assert
            var file = Assert.Single(result);
            Assert.Equal("appsettings.Production.json", file.檔名);
        }

        private static AppSettingsProvider CreateSettings(AppSettings settings)
        {
            var repo = Substitute.For<IAppSettingsRepository>();
            repo.Load().Returns(settings);
            return new AppSettingsProvider(repo);
        }
    }
}
