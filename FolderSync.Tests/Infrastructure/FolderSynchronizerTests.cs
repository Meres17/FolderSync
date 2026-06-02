using FolderSync.Domain;
using FolderSync.Infrastructure;

using NSubstitute;

namespace FolderSync.Tests.Infrastructure
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
            var sut = FakeSynchronizer(FakeBackups());

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
            var sut = FakeSynchronizer(backups);

            // Act
            await sut.ApplyAsync(files, SourceDir, DestDir);

            // Assert
            AssertCopy(sut, $"{BackupDir}\\{SourceDir}", 3);
            AssertCopy(sut, $"{BackupDir}\\{DestDir}", 5);
            AssertCopy(sut, $"{BackupDir}\\{BackupCategory.Delete}", 3);
            AssertCopy(sut, $"{BackupDir}\\{BackupCategory.Diff}", 2);
            AssertCopy(sut, $"{BackupDir}\\{BackupCategory.Add}", 1);

            AssertCopy(sut, DestDir, 3);
            AssertDelete(sut, 3);
        }

        [Fact]
        public async Task RollbackAsync_還原刪除與修改並移除新增()
        {
            // Arrange
            var backups = FakeBackups();
            backups.ReadBackup(Arg.Any<string>(), BackupCategory.Delete).Returns(Snapshots(3));
            backups.ReadBackup(Arg.Any<string>(), BackupCategory.Diff).Returns(Snapshots(2));
            backups.ReadBackup(Arg.Any<string>(), BackupCategory.Add).Returns(Snapshots(1));
            var sut = FakeSynchronizer(backups);

            // Act
            await sut.RollbackAsync(BackupDir, DestDir);

            // Assert
            AssertCopy(sut, DestDir, 3);
            AssertCopy(sut, DestDir, 2);
            AssertDelete(sut, 1);
        }

        private static void AssertCopy(FolderSynchronizer sut, string destHost, int count)
        {
            sut.Received(1).CopyOpsAsync(
                Arg.Is<IEnumerable<CopyOperation>>(x => x.Count() == count),
                destHost);
        }

        private static void AssertDelete(FolderSynchronizer sut, int count)
        {
            sut.Received(1).DeleteAsync(Arg.Is<IEnumerable<string?>>(x => x.Count() == count));
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

        private static FolderSynchronizer FakeSynchronizer(IBackupRepository backups)
        {
            var result = Substitute.For<FolderSynchronizer>(backups);
            // 實際 IO 不執行
            result.When(x => x.CopyOpsAsync(Arg.Any<IEnumerable<CopyOperation>>(), Arg.Any<string>()))
                .DoNotCallBase();
            result.When(x => x.DeleteAsync(Arg.Any<IEnumerable<string?>>()))
                .DoNotCallBase();
            return result;
        }

        private static IBackupRepository FakeBackups()
        {
            return Substitute.For<IBackupRepository>();
        }
    }
}
