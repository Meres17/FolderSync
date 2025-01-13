using FolderSyncCore.Imps;

using NSubstitute;

namespace FolderSyncCore.Tests.UnitTests.Imps
{
    public class SiteControlTests
    {
        private const string DestDir = "destDir";

        [Fact]
        public void CloseSite_目標資料夾不存在時拋出異常()
        {
            // Arrange
            var sut = Substitute.For<SiteControl>();
            sut.NotFoundDirectory(Arg.Any<string>())
                .Returns(true);

            // Act & Assert
            Assert.Throws<DirectoryNotFoundException>(
                () => sut.CloseSite(DestDir));
        }

        [Fact]
        public void CloseSite_找不到App_offline_htm時拋出異常()
        {
            // Arrange
            var sut = Substitute.For<SiteControl>();
            sut.NotFoundDirectory(Arg.Any<string>())
                .Returns(false);

            sut.NotFoundFile(Arg.Any<string>())
               .Returns(true);

            // Act & Assert
            Assert.Throws<Exception>(() => sut.CloseSite(DestDir));
        }

        [Fact]
        public void CloseSite_依照有無App_offline_htm呼叫Delete()
        {
            // Arrange
            var sut = Substitute.For<SiteControl>();
            sut.When(x => x.Copy(Arg.Any<string>(), Arg.Any<string>()))
                .DoNotCallBase();

            sut.NotFoundDirectory(Arg.Any<string>())
                .Returns(false);

            sut.NotFoundFile(Arg.Any<string>())
               .Returns(false);

            // Act
            sut.CloseSite(DestDir);

            // Assert
            sut.Received(1).Copy(Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        public void OpenSite_目標資料夾不存在時拋出異常()
        {
            // Arrange
            var sut = Substitute.For<SiteControl>();
            sut.NotFoundDirectory(Arg.Any<string>())
                .Returns(true);

            // Act & Assert
            Assert.Throws<DirectoryNotFoundException>(
                () => sut.OpenSite(DestDir));
        }

        [Theory]
        [InlineData(false, 1)]
        [InlineData(true, 0)]
        public void OpenSite_依照有無App_offline_htm呼叫Delete(bool hasFile, int received)
        {
            // Arrange
            var sut = Substitute.For<SiteControl>();
            sut.When(x => x.Delete(Arg.Any<string>()))
                .DoNotCallBase();

            sut.NotFoundDirectory(Arg.Any<string>())
                .Returns(false);

            sut.NotFoundFile(Arg.Any<string>())
               .Returns(hasFile);

            // Act
            sut.OpenSite(DestDir);

            // Assert
            sut.Received(received).Delete(Arg.Any<string>());
        }

    }
}
