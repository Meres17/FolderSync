
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
            var sut = new FolderDiffService(scanner, new FolderComparer(), new IgnorePolicy(Array.Empty<string>(), Array.Empty<string>()));

            // Act
            var diff = sut.GetDiff("src", "dst");

            // Assert
            Assert.Single(diff);
            Assert.Equal(CompareState.新增檔案, diff[0].狀態);
        }
    }
}
