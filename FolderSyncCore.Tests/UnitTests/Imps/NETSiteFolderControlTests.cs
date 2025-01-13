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
            ISiteControl stub = FakeSiteControl();
            var sut = new NETSiteFolderControl(mock, stub);

            // Act
            sut.GetFolders(SourceDir, DestDir);

            // Assert
            mock.Received(1)
                .GetFolders(Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        public void Overwrite_調用FolderControl的Overwrite方法()
        {
            // Arrange
            IFolderControl mock = FakeFolderControl();
            ISiteControl stub = FakeSiteControl();
            var sut = new NETSiteFolderControl(mock, stub);

            // Act
            sut.Overwrite(Enumerable.Empty<FileStatus>(), SourceDir, DestDir);

            // Assert
            mock.Received(1)
                .Overwrite(Arg.Any<IEnumerable<FileStatus>>(), Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        public void Restore_調用FolderControl的Restore方法()
        {
            // Arrange
            IFolderControl mock = FakeFolderControl();
            ISiteControl stub = FakeSiteControl();
            var sut = new NETSiteFolderControl(mock, stub);

            // Act
            sut.Restore(BackupDir, DestDir);

            // Assert
            mock.Received(1)
                .Restore(Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        public void Overwrite_調用SiteControl_開關站台()
        {
            // Arrange
            var stub = FakeFolderControl();
            var mock = FakeSiteControl();
            var sut = new NETSiteFolderControl(stub, mock);

            // Act
            sut.Overwrite(new List<FileStatus>(), SourceDir, DestDir);

            // Assert
            mock.Received(1).CloseSite(Arg.Any<string>());
            mock.Received(1).OpenSite(Arg.Any<string>());
        }

        [Fact]
        public void Restore_調用SiteControl_開關站台()
        {
            // Arrange
            var stub = FakeFolderControl();
            var mock = FakeSiteControl();
            var sut = new NETSiteFolderControl(stub, mock);

            // Act
            sut.Restore(BackupDir, DestDir);

            // Assert
            mock.Received(1).CloseSite(Arg.Any<string>());
            mock.Received(1).OpenSite(Arg.Any<string>());
        }

        private static IFolderControl FakeFolderControl()
        {
            return Substitute.For<IFolderControl>();
        }

        private static ISiteControl FakeSiteControl()
        {
            var result = Substitute.For<ISiteControl>();
            result.CloseSite(Arg.Any<string>());
            result.OpenSite(Arg.Any<string>());
            return result;
        }
    }
}
