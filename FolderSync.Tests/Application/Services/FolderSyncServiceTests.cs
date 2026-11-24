
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

        [Fact]
        public void GetDiffFiles_來源空白_拋出請選擇來源資料夾()
        {
            // arrange
            var (sut, _, _, _) = NewService();

            // act
            var ex = Assert.Throws<DomainException>(() => sut.GetDiffFiles(" ", Dst));

            // assert
            Assert.Equal("請選擇來源資料夾", ex.Message);
        }

        [Fact]
        public void GetDiffFiles_目標已被刪除_拋出找不到目標資料夾()
        {
            // arrange
            var (sut, _, _, _) = NewService(ExistOnly(Src));

            // act
            var ex = Assert.Throws<DomainException>(() => sut.GetDiffFiles(Src, Dst));

            // assert
            Assert.Equal("找不到目標資料夾：dst", ex.Message);
        }

        [Fact]
        public void GetDiffFiles_路徑結尾有反斜線_去除後再掃描()
        {
            // arrange
            var (sut, scanner, _, _) = NewService();
            scanner.Scan(Src).Returns(new[] { Snap("a.txt") });
            scanner.Scan(Dst).Returns(Array.Empty<FileSnapshot>());

            // act
            var result = sut.GetDiffFiles(Src + "\\", Dst + "\\");

            // assert
            Assert.Single(result);
        }

        [Fact]
        public void GetIgnoredDiffFiles_來源已被刪除_拋出找不到來源資料夾()
        {
            // arrange
            var (sut, _, _, _) = NewService(ExistOnly(Dst));

            // act
            var ex = Assert.Throws<DomainException>(() => sut.GetIgnoredDiffFiles(Src, Dst));

            // assert
            Assert.Equal("找不到來源資料夾：src", ex.Message);
        }

        [Fact]
        public async Task ApplyAsync_目標已被刪除_不呼叫Synchronizer()
        {
            // arrange
            var (sut, _, factory, _) = NewService(ExistOnly(Src));

            // act
            var ex = await Assert.ThrowsAsync<DomainException>(() => sut.ApplyAsync(Src, Dst, SyncMode.資料夾));

            // assert
            Assert.Equal("找不到目標資料夾：dst", ex.Message);
            factory.DidNotReceive().Create(Arg.Any<SyncMode>());
        }

        [Fact]
        public void GetBackupFolders_來源已被刪除_仍列出備份()
        {
            // arrange
            var (sut, _, _, backups) = NewService(ExistOnly(Dst));
            backups.ListBackups(Src, Dst).Returns(new List<BackupFolder> { new("a") });

            // act
            var result = sut.GetBackupFolders(Src, Dst);

            // assert
            Assert.Single(result);
        }

        [Fact]
        public void GetBackupFolders_來源空白_拋出請選擇來源資料夾()
        {
            // arrange
            var (sut, _, _, backups) = NewService();

            // act
            var ex = Assert.Throws<DomainException>(() => sut.GetBackupFolders("", Dst));

            // assert
            Assert.Equal("請選擇來源資料夾", ex.Message);
            backups.DidNotReceive().ListBackups(Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        public async Task RollbackAsync_備份已被刪除_不呼叫Synchronizer()
        {
            // arrange
            var (sut, _, factory, _) = NewService(ExistOnly(Dst));
            var selected = new List<BackupFolder> { new("C:\\backup\\20230101_1200") };

            // act
            var ex = await Assert.ThrowsAsync<DomainException>(() => sut.RollbackAsync(selected, Dst, SyncMode.資料夾));

            // assert
            Assert.Equal("備份紀錄已不存在：20230101_1200，請重新按「備份紀錄」", ex.Message);
            factory.DidNotReceive().Create(Arg.Any<SyncMode>());
        }

        [Fact]
        public async Task RollbackAsync_目標已被刪除_不呼叫Synchronizer()
        {
            // arrange
            var (sut, _, factory, _) = NewService(ExistOnly("C:\\backup\\20230101_1200"));
            var selected = new List<BackupFolder> { new("C:\\backup\\20230101_1200") };

            // act
            var ex = await Assert.ThrowsAsync<DomainException>(() => sut.RollbackAsync(selected, Dst, SyncMode.資料夾));

            // assert
            Assert.Equal("找不到目標資料夾：dst", ex.Message);
            factory.DidNotReceive().Create(Arg.Any<SyncMode>());
        }

        [Fact]
        public void DeleteBackups_其中一筆已被刪除_全部不刪除()
        {
            // arrange
            var (sut, _, _, backups) = NewService(ExistOnly("a"));
            var selected = new List<BackupFolder> { new("a"), new("b") };

            // act
            var ex = Assert.Throws<DomainException>(() => sut.DeleteBackups(selected));

            // assert
            Assert.Equal("備份紀錄已不存在：b，請重新按「備份紀錄」", ex.Message);
            backups.DidNotReceive().DeleteBackup(Arg.Any<string>());
        }

        private static FileSnapshot Snap(string relativePath)
        {
            return new FileSnapshot(relativePath, "full\\" + relativePath, new DateTime(2023, 1, 1));
        }

        private static (FolderSyncService, IFolderScanner, IFolderSynchronizerFactory, IBackupRepository) NewService(IDirectoryProbe? directories = null)
        {
            var scanner = Substitute.For<IFolderScanner>();
            var factory = Substitute.For<IFolderSynchronizerFactory>();
            var backups = Substitute.For<IBackupRepository>();
            var repo = Substitute.For<IAppSettingsRepository>();
            repo.Load().Returns(new AppSettings());
            var diff = new FolderDiffService(scanner, new FolderComparer(), new AppSettingsProvider(repo));
            var sync = new FolderSynchronizationService(factory, backups, Substitute.For<IAuditLog>());
            var sut = new FolderSyncService(diff, sync, directories ?? AllExist());
            return (sut, scanner, factory, backups);
        }

        private static IDirectoryProbe AllExist()
        {
            var stub = Substitute.For<IDirectoryProbe>();
            stub.Exists(Arg.Any<string>()).Returns(true);
            return stub;
        }

        private static IDirectoryProbe ExistOnly(string path)
        {
            var stub = Substitute.For<IDirectoryProbe>();
            stub.Exists(path).Returns(true);
            return stub;
        }
    }
}
