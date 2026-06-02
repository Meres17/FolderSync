using FolderSync.Domain;
using FolderSync.Infrastructure;

using NSubstitute;

namespace FolderSync.Tests.Infrastructure
{
    public class SiteFolderSynchronizerTests
    {
        private const string SourceDir = "sourceDir";
        private const string DestDir = "destDir";
        private const string BackupDir = "backupDir";

        [Fact]
        public async Task ApplyAsync_調用內層Synchronizer的ApplyAsync()
        {
            // Arrange
            var inner = FakeInner();
            var sut = new SiteFolderSynchronizer(inner, FakeSite(), AppSettings.Empty());

            // Act
            await sut.ApplyAsync(Enumerable.Empty<FileStatus>().ToList(), SourceDir, DestDir);

            // Assert
            await inner.Received(1).ApplyAsync(Arg.Any<IReadOnlyList<FileStatus>>(), Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        public async Task RollbackAsync_調用內層Synchronizer的RollbackAsync()
        {
            // Arrange
            var inner = FakeInner();
            var sut = new SiteFolderSynchronizer(inner, FakeSite(), AppSettings.Empty());

            // Act
            await sut.RollbackAsync(BackupDir, DestDir);

            // Assert
            await inner.Received(1).RollbackAsync(Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        public async Task ApplyAsync_透過WithSiteOffline操作站台()
        {
            // Arrange
            var sut = Substitute.For<SiteFolderSynchronizer>(FakeInner(), FakeSite(), AppSettings.Empty());

            // Act
            await sut.ApplyAsync(new List<FileStatus>(), SourceDir, DestDir);

            // Assert
            await sut.Received(1).WithSiteOfflineAsync(Arg.Any<string>(), Arg.Any<Func<Task>>());
        }

        [Fact]
        public async Task RollbackAsync_透過WithSiteOffline操作站台()
        {
            // Arrange
            var sut = Substitute.For<SiteFolderSynchronizer>(FakeInner(), FakeSite(), AppSettings.Empty());

            // Act
            await sut.RollbackAsync(BackupDir, DestDir);

            // Assert
            await sut.Received(1).WithSiteOfflineAsync(Arg.Any<string>(), Arg.Any<Func<Task>>());
        }

        [Fact]
        public async Task WithSiteOffline_先離線再上線()
        {
            // Arrange
            var site = FakeSite();
            var sut = new SiteFolderSynchronizer(FakeInner(), site, AppSettings.Empty());

            // Act
            await sut.WithSiteOfflineAsync(DestDir, () => Task.CompletedTask);

            // Assert
            site.Received(1).TakeOffline(Arg.Any<string>());
            site.Received(1).BringOnline(Arg.Any<string>());
        }

        [Fact]
        public async Task WithSiteOffline_離線失敗仍會上線()
        {
            // Arrange
            var site = FakeSite();
            var sut = new SiteFolderSynchronizer(FakeInner(), site, AppSettings.Empty());
            site.When(x => x.TakeOffline(Arg.Any<string>())).Do(x => throw new Exception("離線失敗"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(
                () => sut.WithSiteOfflineAsync(DestDir, () => Task.CompletedTask));
            site.Received(1).BringOnline(Arg.Any<string>());
        }

        [Fact]
        public async Task WithSiteOffline_操作失敗仍會上線()
        {
            // Arrange
            var site = FakeSite();
            var sut = new SiteFolderSynchronizer(FakeInner(), site, AppSettings.Empty());

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(
                () => sut.WithSiteOfflineAsync(DestDir, () => throw new Exception("任何例外")));
            site.Received(1).BringOnline(Arg.Any<string>());
        }

        private static IFolderSynchronizer FakeInner()
        {
            return Substitute.For<IFolderSynchronizer>();
        }

        private static ISiteController FakeSite()
        {
            return Substitute.For<ISiteController>();
        }
    }
}
