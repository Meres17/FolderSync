
using NSubstitute;

namespace FolderSync.Tests.Application.Services
{
    public class FolderSynchronizationServiceTests
    {
        private const string Src = "src";
        private const string Dst = "dst";

        [Fact]
        public void Modes_委派給Factory()
        {
            // Arrange
            var (sut, factory, _) = New();
            factory.Modes.Returns(new[] { SyncMode.資料夾 });

            // Act & Assert
            Assert.Equal(new[] { SyncMode.資料夾 }, sut.Modes);
        }

        [Fact]
        public async Task ApplyAsync_依模式建立Synchronizer並套用()
        {
            // Arrange
            var (sut, factory, _) = New();
            var synchronizer = Substitute.For<IFolderSynchronizer>();
            factory.Create(SyncMode.資料夾).Returns(synchronizer);
            var changes = new List<FileStatus>();

            // Act
            await sut.ApplyAsync(changes, Src, Dst, SyncMode.資料夾);

            // Assert
            await synchronizer.Received(1).ApplyAsync(changes, Src, Dst);
        }

        [Fact]
        public async Task RollbackAsync_依模式建立Synchronizer並還原()
        {
            // Arrange
            var (sut, factory, _) = New();
            var synchronizer = Substitute.For<IFolderSynchronizer>();
            factory.Create(SyncMode.站台).Returns(synchronizer);

            // Act
            await sut.RollbackAsync("backupDir", Dst, SyncMode.站台);

            // Assert
            await synchronizer.Received(1).RollbackAsync("backupDir", Dst);
        }

        [Fact]
        public void ListBackups_委派給Backups()
        {
            // Arrange
            var (sut, _, backups) = New();
            backups.ListBackups(Src, Dst).Returns(new List<BackupFolder> { new("a") });

            // Act & Assert
            Assert.Single(sut.ListBackups(Src, Dst));
        }

        [Fact]
        public void DeleteBackup_委派給Backups()
        {
            // Arrange
            var (sut, _, backups) = New();

            // Act
            sut.DeleteBackup("a");

            // Assert
            backups.Received(1).DeleteBackup("a");
        }

        [Fact]
        public async Task ApplyAsync_失敗_記錄錯誤並重拋()
        {
            // Arrange
            var factory = Substitute.For<IFolderSynchronizerFactory>();
            var backups = Substitute.For<IBackupRepository>();
            var log = Substitute.For<IAuditLog>();
            var synchronizer = Substitute.For<IFolderSynchronizer>();
            synchronizer.ApplyAsync(Arg.Any<IReadOnlyList<FileStatus>>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(Task.FromException(new IOException("磁碟錯誤")));
            factory.Create(SyncMode.資料夾).Returns(synchronizer);
            var sut = new FolderSynchronizationService(factory, backups, log);

            // Act & Assert
            await Assert.ThrowsAsync<IOException>(
                () => sut.ApplyAsync(new List<FileStatus>(), Src, Dst, SyncMode.資料夾));
            log.Received(1).Error(Arg.Any<string>(), Arg.Any<IOException>());
        }

        private static (FolderSynchronizationService, IFolderSynchronizerFactory, IBackupRepository) New()
        {
            var factory = Substitute.For<IFolderSynchronizerFactory>();
            var backups = Substitute.For<IBackupRepository>();
            var sut = new FolderSynchronizationService(factory, backups, Substitute.For<IAuditLog>());
            return (sut, factory, backups);
        }
    }
}
