using FolderSyncCore.Imps;

using NSubstitute;

namespace FolderSyncCore.Tests.UnitTests.Imps
{
    public class NETSiteFolderControlTests
    {
        private const string SourceDir = "sourceDir";
        private const string DestDir = "destDir";
        private const string BackupDir = "backupDir";

        [Fact]
        public void GetFolders_調用FolderControl的GetFolders方法()
        {
            // Arrange
            IFolderControl mock = FakeFolderControl();
            var sut = new NETSiteFolderControl(mock);

            // Act
            sut.GetFolders(SourceDir, DestDir);

            // Assert
            mock.Received(1).GetFolders(Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        public void Overwrite_關閉站台_調用FolderControl的Overwrite_打開站台()
        {
            // Arrange
            var mock = FakeFolderControl();
            var sut = Substitute.For<NETSiteFolderControl>(mock);
            sut.CloseSite(Arg.Any<string>());
            sut.OpenSite(Arg.Any<string>());

            var stub = new List<FileStatus>();

            // Act
            sut.Overwrite(stub, SourceDir, DestDir);

            // Assert
            sut.Received(1).CloseSite(Arg.Any<string>());
            mock.Received(1).Overwrite(stub, Arg.Any<string>(), Arg.Any<string>());
            sut.Received(1).OpenSite(Arg.Any<string>());
        }

        [Fact]
        public void Restore_關閉站台_調用FolderControl的Restore_打開站點()
        {
            // Arrange
            var mock = FakeFolderControl();
            var sut = Substitute.For<NETSiteFolderControl>(mock);
            sut.CloseSite(Arg.Any<string>());
            sut.OpenSite(Arg.Any<string>());

            // Act
            sut.Restore(BackupDir, DestDir);

            // Assert
            mock.Received(1).Restore(Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        public void CloseSite_目標資料夾不存在時拋出異常()
        {
            // Arrange
            var stub = FakeFolderControl();
            var sut = Substitute.ForPartsOf<NETSiteFolderControl>(stub);
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
            var stub = FakeFolderControl();
            var sut = Substitute.ForPartsOf<NETSiteFolderControl>(stub);
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
            var stub = FakeFolderControl();
            var sut = Substitute.ForPartsOf<NETSiteFolderControl>(stub);
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
            var stub = FakeFolderControl();
            var sut = Substitute.ForPartsOf<NETSiteFolderControl>(stub);
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
            var stub = FakeFolderControl();
            var sut = Substitute.ForPartsOf<NETSiteFolderControl>(stub);
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

        private static IFolderControl FakeFolderControl()
        {
            return Substitute.For<IFolderControl>();
        }
    }
}
