using NSubstitute;

namespace FolderSync.Tests.Infrastructure.Synchronizers
{
    public class FolderSynchronizerTests
    {
        private const string SourceDir = "sourceDir";
        private const string DestDir = "destDir";
        private const string BackupDir = "backup";

        [Fact]
        public async Task ApplyAsync_沒有檔案_拋出DomainException()
        {
            // Arrange
            var sut = new FolderSynchronizer(FakeBackups(), FakeFiles());

            // Act & Assert
            await Assert.ThrowsAsync<DomainException>(
                () => sut.ApplyAsync(new List<FileStatus>(), SourceDir, DestDir));
        }

        [Fact]
        public async Task ApplyAsync_依狀態複製刪除並建立各備份分類()
        {
            // Arrange
            var files = new List<FileStatus>
            {
                FakeFileStatus(CompareState.新增檔案),
                FakeFileStatus(CompareState.時間不同),
                FakeFileStatus(CompareState.時間不同),
                FakeFileStatus(CompareState.刪除檔案),
                FakeFileStatus(CompareState.刪除檔案),
                FakeFileStatus(CompareState.刪除檔案),
            };
            var backups = FakeBackups();
            backups.CreateBackupDirectory(Arg.Any<string>(), Arg.Any<string>()).Returns(BackupDir);
            var ops = FakeFiles();
            var sut = new FolderSynchronizer(backups, ops);

            // Act
            await sut.ApplyAsync(files, SourceDir, DestDir);

            // Assert
            AssertCopy(ops, $"{BackupDir}\\{SourceDir}", 3);
            AssertCopy(ops, $"{BackupDir}\\{DestDir}", 5);
            AssertCopy(ops, $"{BackupDir}\\{BackupCategory.Delete}", 3);
            AssertCopy(ops, $"{BackupDir}\\{BackupCategory.Diff}", 2);
            AssertCopy(ops, $"{BackupDir}\\{BackupCategory.Add}", 1);

            AssertCopy(ops, DestDir, 3);
            AssertDelete(ops, 3);
        }

        [Fact]
        public async Task RollbackAsync_還原刪除與修改並移除新增()
        {
            // Arrange
            var backups = FakeBackups();
            backups.ReadBackup(Arg.Any<string>(), BackupCategory.Delete).Returns(Snapshots(3));
            backups.ReadBackup(Arg.Any<string>(), BackupCategory.Diff).Returns(Snapshots(2));
            backups.ReadBackup(Arg.Any<string>(), BackupCategory.Add).Returns(Snapshots(1));
            var ops = FakeFiles();
            var sut = new FolderSynchronizer(backups, ops);

            // Act
            await sut.RollbackAsync(BackupDir, DestDir);

            // Assert
            AssertCopy(ops, DestDir, 3);
            AssertCopy(ops, DestDir, 2);
            AssertDelete(ops, 1);
        }

        [Fact]
        public async Task ApplyAsync_套用失敗_自動還原並重拋()
        {
            // Arrange
            var backups = FakeBackups();
            backups.CreateBackupDirectory(Arg.Any<string>(), Arg.Any<string>()).Returns(BackupDir);
            backups.ReadBackup(Arg.Any<string>(), Arg.Any<string>()).Returns(Array.Empty<FileSnapshot>());
            var ops = FakeFiles();
            // 僅前向套用（複製非空變更到目標）失敗 → 觸發自動還原；還原是空集合複製，可成功
            ops.When(x => x.CopyAsync(Arg.Is<IEnumerable<CopyOperation>>(c => c.Any()), DestDir))
               .Do(_ => throw new IOException("磁碟錯誤"));
            var sut = new FolderSynchronizer(backups, ops);
            var files = new List<FileStatus> { FakeFileStatus(CompareState.新增檔案) };

            // Act & Assert
            await Assert.ThrowsAsync<IOException>(() => sut.ApplyAsync(files, SourceDir, DestDir));

            // 還原被觸發：讀取備份分類以回復目標
            backups.Received().ReadBackup(BackupDir, BackupCategory.Delete);
        }

        private static void AssertCopy(IFileOperations files, string destHost, int count)
        {
            files.Received(1).CopyAsync(
                Arg.Is<IEnumerable<CopyOperation>>(x => x.Count() == count),
                destHost);
        }

        private static void AssertDelete(IFileOperations files, int count)
        {
            files.Received(1).DeleteAsync(Arg.Is<IEnumerable<string?>>(x => x.Count() == count));
        }

        private static IReadOnlyList<FileSnapshot> Snapshots(int count)
        {
            return Enumerable.Range(0, count)
                .Select(i => new FileSnapshot($"file{i}.txt", $"full\\file{i}.txt", new DateTime(2023, 1, 1)))
                .ToList();
        }

        private static FileStatus FakeFileStatus(CompareState state)
        {
            var result = Substitute.For<FileStatus>();
            result.狀態.Returns(state);
            result.相對路徑.Returns("path");
            return result;
        }

        private static IFileOperations FakeFiles()
        {
            return Substitute.For<IFileOperations>();
        }

        private static IBackupRepository FakeBackups()
        {
            return Substitute.For<IBackupRepository>();
        }
    }
}
