using FolderSync.Application;
using FolderSync.Domain;

using NSubstitute;

namespace FolderSync.Tests.Application
{
    public class FolderSyncServiceTests
    {
        private const string SourceDir = "sourceDir";
        private const string DestDir = "destDir";
        private const string Type = "資料夾";

        [Fact]
        public void GetNames_委派給Factory()
        {
            // Arrange
            var reader = Substitute.For<IFolderReader>();
            var factory = Substitute.For<IFolderControlFactory>();
            factory.GetNames().Returns(new[] { "a", "b" });
            var sut = new FolderSyncService(reader, factory);

            // Act
            var result = sut.GetNames();

            // Assert
            Assert.Equal(new[] { "a", "b" }, result);
        }

        [Fact]
        public void GetDiffFiles_委派給Reader()
        {
            // Arrange
            var reader = Substitute.For<IFolderReader>();
            var factory = Substitute.For<IFolderControlFactory>();
            var files = new List<FileStatus>();
            reader.GetDiffFiles(SourceDir, DestDir).Returns(files);
            var sut = new FolderSyncService(reader, factory);

            // Act
            var result = sut.GetDiffFiles(SourceDir, DestDir);

            // Assert
            Assert.Same(files, result);
        }

        [Fact]
        public void Overwrite_取得差異後呼叫Control的Overwrite()
        {
            // Arrange
            var reader = Substitute.For<IFolderReader>();
            var factory = Substitute.For<IFolderControlFactory>();
            var control = Substitute.For<IFolderControl>();
            var files = new List<FileStatus>();
            reader.GetDiffFiles(SourceDir, DestDir).Returns(files);
            factory.CreateControl(Type).Returns(control);
            var sut = new FolderSyncService(reader, factory);

            // Act
            sut.Overwrite(SourceDir, DestDir, Type);

            // Assert
            control.Received(1).Overwrite(files, SourceDir, DestDir);
        }

        [Fact]
        public void Restore_沒有選取_拋出例外()
        {
            // Arrange
            var sut = NewService(out _, out _, out _);

            // Act & Assert
            Assert.Throws<Exception>(() => sut.Restore(new List<BackupFolder>(), DestDir, Type));
        }

        [Fact]
        public void Restore_選取多筆_拋出例外()
        {
            // Arrange
            var sut = NewService(out _, out _, out _);
            var selected = new List<BackupFolder> { new("a"), new("b") };

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => sut.Restore(selected, DestDir, Type));
            Assert.Equal("要還原的檔案只能選一個", ex.Message);
        }

        [Fact]
        public void Restore_選取一筆_呼叫Control的Restore()
        {
            // Arrange
            var sut = NewService(out _, out var factory, out var control);
            factory.CreateControl(Type).Returns(control);
            var selected = new List<BackupFolder> { new("C:\\backup\\20230101_1200") };

            // Act
            sut.Restore(selected, DestDir, Type);

            // Assert
            control.Received(1).Restore("C:\\backup\\20230101_1200", DestDir);
        }

        [Fact]
        public void DeleteBackups_沒有選取_拋出例外()
        {
            // Arrange
            var sut = NewService(out _, out _, out _);

            // Act & Assert
            Assert.Throws<Exception>(() => sut.DeleteBackups(new List<BackupFolder>()));
        }

        [Fact]
        public void DeleteBackups_逐筆刪除()
        {
            // Arrange
            var sut = NewService(out var reader, out _, out _);
            var selected = new List<BackupFolder> { new("a"), new("b") };

            // Act
            sut.DeleteBackups(selected);

            // Assert
            reader.Received(1).DeleteBackupFolder("a");
            reader.Received(1).DeleteBackupFolder("b");
        }

        private static FolderSyncService NewService(out IFolderReader reader, out IFolderControlFactory factory, out IFolderControl control)
        {
            reader = Substitute.For<IFolderReader>();
            factory = Substitute.For<IFolderControlFactory>();
            control = Substitute.For<IFolderControl>();
            return new FolderSyncService(reader, factory);
        }
    }
}
