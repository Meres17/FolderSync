
using NSubstitute;

namespace FolderSync.Tests.Application.Services
{
    public class FolderSyncServiceTests
    {
        private const string Src = "src";
        private const string Dst = "dst";

        [Fact]
        public void GetSyncModes_委派給Factory()
        {
            // Arrange
            var (sut, _, factory, _) = NewService();
            factory.Modes.Returns(new[] { SyncMode.資料夾, SyncMode.站台 });

            // Act & Assert
            Assert.Equal(new[] { SyncMode.資料夾, SyncMode.站台 }, sut.GetSyncModes());
        }

        [Fact]
        public void GetDiffFiles_掃描來源與目標後比對產生差異()
        {
            // Arrange
            var (sut, scanner, _, _) = NewService();
            scanner.Scan(Src).Returns(new[] { Snap("a.txt") });
            scanner.Scan(Dst).Returns(Array.Empty<FileSnapshot>());

            // Act
            var diff = sut.GetDiffFiles(Src, Dst);

            // Assert
            Assert.Single(diff);
            Assert.Equal(CompareState.新增檔案, diff[0].狀態);
        }

        [Fact]
        public async Task ApplyAsync_取得差異後呼叫Synchronizer的ApplyAsync()
        {
            // Arrange
            var (sut, scanner, factory, _) = NewService();
            scanner.Scan(Src).Returns(new[] { Snap("a.txt") });
            scanner.Scan(Dst).Returns(Array.Empty<FileSnapshot>());
            var sync = Substitute.For<IFolderSynchronizer>();
            factory.Create(SyncMode.資料夾).Returns(sync);

            // Act
            await sut.ApplyAsync(Src, Dst, SyncMode.資料夾);

            // Assert
            await sync.Received(1).ApplyAsync(Arg.Is<IReadOnlyList<FileStatus>>(x => x.Count == 1), Src, Dst);
        }

        [Fact]
        public async Task RollbackAsync_選取一筆_呼叫Synchronizer的RollbackAsync()
        {
            // Arrange
            var (sut, _, factory, _) = NewService();
            var sync = Substitute.For<IFolderSynchronizer>();
            factory.Create(SyncMode.站台).Returns(sync);
            var selected = new List<BackupFolder> { new("C:\\backup\\20230101_1200") };

            // Act
            await sut.RollbackAsync(selected, Dst, SyncMode.站台);

            // Assert
            await sync.Received(1).RollbackAsync("C:\\backup\\20230101_1200", Dst);
        }

        [Fact]
        public async Task RollbackAsync_沒有選取_拋出DomainException()
        {
            // Arrange
            var (sut, _, _, _) = NewService();

            // Act & Assert
            await Assert.ThrowsAsync<DomainException>(
                () => sut.RollbackAsync(new List<BackupFolder>(), Dst, SyncMode.資料夾));
        }

        [Fact]
        public async Task RollbackAsync_選取多筆_拋出DomainException()
        {
            // Arrange
            var (sut, _, _, _) = NewService();
            var selected = new List<BackupFolder> { new("a"), new("b") };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<DomainException>(
                () => sut.RollbackAsync(selected, Dst, SyncMode.資料夾));
            Assert.Equal("要還原的檔案只能選一個", ex.Message);
        }

        [Fact]
        public void DeleteBackups_逐筆刪除()
        {
            // Arrange
            var (sut, _, _, backups) = NewService();
            var selected = new List<BackupFolder> { new("a"), new("b") };

            // Act
            sut.DeleteBackups(selected);

            // Assert
            backups.Received(1).DeleteBackup("a");
            backups.Received(1).DeleteBackup("b");
        }

        [Fact]
        public void DeleteBackups_沒有選取_拋出DomainException()
        {
            // Arrange
            var (sut, _, _, _) = NewService();

            // Act & Assert
            Assert.Throws<DomainException>(() => sut.DeleteBackups(new List<BackupFolder>()));
        }

        [Fact]
        public void GetBackupFolders_委派給Backups()
        {
            // Arrange
            var (sut, _, _, backups) = NewService();
            backups.ListBackups(Src, Dst).Returns(new List<BackupFolder> { new("a") });

            // Act & Assert
            Assert.Single(sut.GetBackupFolders(Src, Dst));
        }

        private static FileSnapshot Snap(string relativePath)
        {
            return new FileSnapshot(relativePath, "full\\" + relativePath, new DateTime(2023, 1, 1));
        }

        private static (FolderSyncService, IFolderScanner, IFolderSynchronizerFactory, IBackupRepository) NewService()
        {
            var scanner = Substitute.For<IFolderScanner>();
            var factory = Substitute.For<IFolderSynchronizerFactory>();
            var backups = Substitute.For<IBackupRepository>();
            var repo = Substitute.For<IAppSettingsRepository>();
            repo.Load().Returns(new AppSettings());
            var diff = new FolderDiffService(scanner, new FolderComparer(), new AppSettingsProvider(repo));
            var sync = new FolderSynchronizationService(factory, backups);
            var sut = new FolderSyncService(diff, sync);
            return (sut, scanner, factory, backups);
        }
    }
}
