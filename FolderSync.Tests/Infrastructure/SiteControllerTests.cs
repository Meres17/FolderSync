using FolderSync.Infrastructure;

using NSubstitute;

namespace FolderSync.Tests.Infrastructure
{
    public class SiteControllerTests
    {
        private const string SiteDir = "siteDir";

        [Fact]
        public void TakeOffline_站台資料夾不存在_拋出異常()
        {
            // Arrange
            var sut = Substitute.For<SiteController>();
            sut.NotFoundDirectory(Arg.Any<string>()).Returns(true);

            // Act & Assert
            Assert.Throws<DirectoryNotFoundException>(() => sut.TakeOffline(SiteDir));
        }

        [Fact]
        public void TakeOffline_找不到App_offline_htm_拋出異常()
        {
            // Arrange
            var sut = Substitute.For<SiteController>();
            sut.NotFoundDirectory(Arg.Any<string>()).Returns(false);
            sut.NotFoundFile(Arg.Any<string>()).Returns(true);

            // Act & Assert
            Assert.Throws<FileNotFoundException>(() => sut.TakeOffline(SiteDir));
        }

        [Fact]
        public void TakeOffline_條件齊備時複製離線檔()
        {
            // Arrange
            var sut = Substitute.For<SiteController>();
            sut.When(x => x.Copy(Arg.Any<string>(), Arg.Any<string>())).DoNotCallBase();
            sut.NotFoundDirectory(Arg.Any<string>()).Returns(false);
            sut.NotFoundFile(Arg.Any<string>()).Returns(false);

            // Act
            sut.TakeOffline(SiteDir);

            // Assert
            sut.Received(1).Copy(Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        public void BringOnline_站台資料夾不存在_拋出異常()
        {
            // Arrange
            var sut = Substitute.For<SiteController>();
            sut.NotFoundDirectory(Arg.Any<string>()).Returns(true);

            // Act & Assert
            Assert.Throws<DirectoryNotFoundException>(() => sut.BringOnline(SiteDir));
        }

        [Theory]
        [InlineData(false, 1)]
        [InlineData(true, 0)]
        public void BringOnline_依有無離線檔決定是否刪除(bool offlineMissing, int received)
        {
            // Arrange
            var sut = Substitute.For<SiteController>();
            sut.When(x => x.Delete(Arg.Any<string>())).DoNotCallBase();
            sut.NotFoundDirectory(Arg.Any<string>()).Returns(false);
            sut.NotFoundFile(Arg.Any<string>()).Returns(offlineMissing);

            // Act
            sut.BringOnline(SiteDir);

            // Assert
            sut.Received(received).Delete(Arg.Any<string>());
        }
    }
}
