using FolderSyncCore.Imps;

using NSubstitute;

namespace FolderSyncCore.Tests.UnitTests.Imps
{
    public class AsyncFolderControlTests
    {
        private const string DestDir = "destDir";
        private const string SourceDir = "sourceDir";
        private const string BackupDir = "backup";

        [Fact]
        public async Task OverwriteAsync_沒有檔案彈出ArgumentException()
        {
            // Arrange
            var files = Enumerable.Empty<FileStatus>();

            var stub = FakeFolderReader();
            AsyncFolderControl sut = FakeFolderControl(stub);

            // Act
            // Assert

            await Assert.ThrowsAsync<ArgumentException>(() => sut.OverwriteAsync(files, SourceDir, DestDir));
        }

        [Fact]
        public async Task OverwriteAsync_測試不同狀態的資料進行了Copy和Delete()
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
            var stub = FakeFolderReader();
            AsyncFolderControl sut = FakeFolderControl(stub);

            stub.CreateBackupDirectory(Arg.Any<string>(), Arg.Any<string>())
                .Returns(BackupDir);

            // Act
            await sut.OverwriteAsync(files, SourceDir, DestDir);

            // Assert
            await Assert_Copy_Received(sut, $"{BackupDir}\\{SourceDir}", 3);
            await Assert_Copy_Received(sut, $"{BackupDir}\\{DestDir}", 5);
            await Assert_Copy_Received(sut, $"{BackupDir}\\{FolderControl.DeleteName}", 3);
            await Assert_Copy_Received(sut, $"{BackupDir}\\{FolderControl.DiffName}", 2);
            await Assert_Copy_Received(sut, $"{BackupDir}\\{FolderControl.AddName}", 1);

            await Assert_Copy_Received(sut, DestDir, 3);
            await Assert_Delete_Received(sut, 3);
        }

        [Fact]
        public async Task RestoreAsync_測試不同狀態的資料進行了Copy和Delete()
        {
            // Arrange
            var stub = FakeFolderReader();
            AsyncFolderControl sut = FakeFolderControl(stub);

            var three_data = DeleteFiles_3_Data();
            stub.GetBackupFiles(Arg.Any<string>(), FolderControl.DeleteName)
                .Returns(three_data);

            var two_data = DiffFiles_2_Data();
            stub.GetBackupFiles(Arg.Any<string>(), FolderControl.DiffName)
                .Returns(two_data);

            var one_data = AddFiles_1_data();
            stub.GetBackupFiles(Arg.Any<string>(), FolderControl.AddName)
                .Returns(one_data);

            // Act
            await sut.RestoreAsync(BackupDir, DestDir);

            // Assert
            await Assert_Copy_Received(sut, DestDir, 3);
            await Assert_Copy_Received(sut, DestDir, 2);

            await Assert_Delete_Received(sut, 1);
        }


        /// <summary>
        /// 驗證 Copy 特定目錄接收到的檔案數量
        /// </summary>
        /// <param name="sut"></param>
        /// <param name="path"></param>
        /// <param name="count"></param>
        private static Task Assert_Copy_Received(AsyncFolderControl sut, string path, int count)
        {
            return sut.Received(1)
                .CopyAsync(
                Arg.Is<IEnumerable<FileStatus>>(x => x.Count() == count),
                path,
                Arg.Any<Func<FileStatus, string>>());
        }

        /// <summary>
        /// 驗證 Delete 接收到的檔案數量
        /// </summary>
        /// <param name="sut"></param>
        /// <param name="count"></param>
        private static Task Assert_Delete_Received(AsyncFolderControl sut, int count)
        {
            return sut.Received(1)
                .DeleteAsync(
                Arg.Is<IEnumerable<FileStatus>>(x => x.Count() == count),
                Arg.Any<Func<FileStatus, string>>());
        }

        private static List<FileStatus> AddFiles_1_data()
        {
            return new List<FileStatus>
            {
                FakeFileStatus(CompareState.新增檔案),
            };
        }

        private static List<FileStatus> DiffFiles_2_Data()
        {
            return new List<FileStatus>
            {
                FakeFileStatus(CompareState.時間不同),
                FakeFileStatus(CompareState.時間不同),
            };
        }

        private static List<FileStatus> DeleteFiles_3_Data()
        {
            return new List<FileStatus>
            {
                FakeFileStatus(CompareState.刪除檔案),
                FakeFileStatus(CompareState.刪除檔案),
                FakeFileStatus(CompareState.刪除檔案),
            };
        }

        private static FileStatus FakeFileStatus(CompareState state)
        {
            var result = Substitute.For<FileStatus>();
            result.狀態.Returns(state);
            result.相對路徑.Returns("path");
            return result;
        }

        private static AsyncFolderControl FakeFolderControl(IFolderReader reader)
        {
            var result = Substitute.For<AsyncFolderControl>(reader);
            // Copy與Delete不執行實際動作
            result.When(x =>
            x.CopyAsync(Arg.Any<IEnumerable<FileStatus>>(),
            Arg.Any<string>(),
            Arg.Any<Func<FileStatus, string>>()))
                .DoNotCallBase();

            result.When(x =>
            x.DeleteAsync(Arg.Any<IEnumerable<FileStatus>>(),
            Arg.Any<Func<FileStatus, string>>()))
                .DoNotCallBase();
            return result;
        }

        private static IFolderReader FakeFolderReader()
        {
            return Substitute.For<IFolderReader>();
        }
    }
}
