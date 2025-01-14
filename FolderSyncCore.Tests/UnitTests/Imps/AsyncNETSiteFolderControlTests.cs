using FolderSyncCore.Imps;

using NSubstitute;

namespace FolderSyncCore.Tests.UnitTests.Imps
{
    public class AsyncNETSiteFolderControlTests
    {
        private const string SourceDir = "sourceDir";
        private const string DestDir = "destDir";
        private const string BackupDir = "backupDir";

        [Fact]
        public async Task Overwrite_調用FolderControl的Overwrite方法()
        {
            // Arrange
            var mock = FakeFolderControl();
            ISiteControl stub = FakeSiteControl();
            var sut = new AsyncNETSiteFolderControl(mock, stub);

            // Act
            await sut.OverwriteAsync(Enumerable.Empty<FileStatus>(), SourceDir, DestDir);

            // Assert
            await mock
                .Received(1)
                .OverwriteAsync(
                Arg.Any<IEnumerable<FileStatus>>(),
                Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        public async Task Restore_調用FolderControl的Restore方法()
        {
            // Arrange
            var mock = FakeFolderControl();
            ISiteControl stub = FakeSiteControl();
            var sut = new AsyncNETSiteFolderControl(mock, stub);

            // Act
            await sut.RestoreAsync(BackupDir, DestDir);

            // Assert
            await mock.Received(1)
                .RestoreAsync(Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        public async Task Overwrite_調用ExecuteWithSiteControl操作站台()
        {
            // Arrange
            var stubFolder = FakeFolderControl();
            var stubSite = FakeSiteControl();
            var sut = Substitute.For<AsyncNETSiteFolderControl>(stubFolder, stubSite);

            // Act
            await sut.OverwriteAsync(new List<FileStatus>(), SourceDir, DestDir);

            // Assert
            await sut.Received(1)
                .ExecuteWithSiteControl(Arg.Any<string>(), Arg.Any<Func<Task>>());
        }

        [Fact]
        public async Task Restore_調用ExecuteWithSiteControl操作站台()
        {
            // Arrange
            var stubFolder = FakeFolderControl();
            var stubSite = FakeSiteControl();
            var sut = Substitute.For<AsyncNETSiteFolderControl>(stubFolder, stubSite);

            // Act
            await sut.RestoreAsync(BackupDir, DestDir);

            // Assert
            await sut.Received(1)
                .ExecuteWithSiteControl(Arg.Any<string>(), Arg.Any<Func<Task>>());
        }

        [Fact]
        public async Task ExecuteWithSiteControl_調用SiteControl_開關站台()
        {
            // Arrange
            var stub = FakeFolderControl();
            var mock = FakeSiteControl();
            var sut = new AsyncNETSiteFolderControl(stub, mock);

            // Act
            await sut.ExecuteWithSiteControl(DestDir, () => Task.CompletedTask);

            // Assert
            mock.Received(1).CloseSite(Arg.Any<string>());
            mock.Received(1).OpenSite(Arg.Any<string>());
        }

        [Fact]
        public async Task ExecuteWithSiteControl_當關閉站台出現例外_開啟站台()
        {
            // Arrange
            var stub = FakeFolderControl();
            var mock = FakeSiteControl();
            var sut = new AsyncNETSiteFolderControl(stub, mock);
            mock.When(x => x.CloseSite(Arg.Any<string>()))
                .Do(x => { throw new Exception("關閉站台失敗"); });

            // Act
            // Assert
            await Assert.ThrowsAsync<Exception>(
                () => sut.ExecuteWithSiteControl(DestDir,
                () => Task.CompletedTask));
            mock.Received(1).OpenSite(Arg.Any<string>());
        }

        [Fact]
        public async Task ExecuteWithSiteControl_當執行操作出現例外_開啟站台()
        {
            // Arrange
            var stub = FakeFolderControl();
            var mock = FakeSiteControl();
            var sut = new AsyncNETSiteFolderControl(stub, mock);

            // Act
            // Assert
            await Assert.ThrowsAsync<Exception>(() => sut.ExecuteWithSiteControl(DestDir,
                () => throw new Exception("任何例外")));
            mock.Received(1).OpenSite(Arg.Any<string>());
        }

        private static IAsyncFolderControl FakeFolderControl()
        {
            return Substitute.For<IAsyncFolderControl>();
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
