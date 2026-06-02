using FolderSync.Application;
using FolderSync.Domain;

using NSubstitute;

namespace FolderSync.Tests.Application
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
            backups.Received(1).Delete("a");
        }

        private static (FolderSynchronizationService, IFolderSynchronizerFactory, IBackupRepository) New()
        {
            var factory = Substitute.For<IFolderSynchronizerFactory>();
            var backups = Substitute.For<IBackupRepository>();
            var sut = new FolderSynchronizationService(factory, backups);
            return (sut, factory, backups);
        }
    }
}
