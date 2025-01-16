using FolderSyncCore.Imps;

namespace FolderSyncCore.Tests.UnitTests.Imps
{
    public class FolderReaderTests
    {
        [Fact]
        public void GetPathDictionary_無效路徑_拋出DirectoryNotFoundException()
        {
            // Arrange
            var stub = AppSettings.Empty();
            var sut = new FolderReader(stub);

            // Act & Assert
            Assert.Throws<DirectoryNotFoundException>(() => sut.GetPathDictionary("anypath"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void GetPathDictionary_空白路徑_拋出ArgumentException(string dir)
        {
            // Arrange
            var stub = AppSettings.Empty();
            var sut = new FolderReader(stub);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => sut.GetPathDictionary(dir));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void GetBackupFolders_空白路徑_拋出ArgumentException(string dir)
        {
            // Arrange
            var stub = AppSettings.Empty();
            var sut = new FolderReader(stub);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => sut.GetBackupFiles("", dir));
        }

        [Fact]
        public void GetBackupFolders_無效路徑_取得空資料()
        {
            // Arrange
            var stub = AppSettings.Empty();
            var sut = new FolderReader(stub);

            // Act
            var result = sut.GetBackupFiles("anypath", "");

            // Assert
            Assert.Empty(result);
        }

        [Theory]
        [InlineData("C:\\path\\to\\ignore.txt", true)]
        [InlineData("C:\\path\\to\\ignore.doc", true)]
        [InlineData("C:\\path\\to\\ignore.pdf", false)]//副檔名不是忽略清單
        [InlineData("C:\\path\\ignore.txt\\file.txt", false)]//路徑中有ignore.txt
        [InlineData("C:\\path\\to\\file.txt", false)]//完全不相同
        public void IsIgnoreFile_檔案路徑中有沒有指定檔名(string path, bool expected)
        {
            // Arrange
            var stub = AppSettings.Empty();
            stub.IgnoreFiles = new[] { "ignore.txt", "ignore.doc" };

            var sut = new FolderReader(stub);

            // Act
            var result = sut.IsIgnoreFile(path, stub.IgnoreFiles);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("bin\\file.txt", true)]
        [InlineData("obj\\file.txt", true)]
        [InlineData("src\\bin\\file.txt", true)]
        [InlineData("src\\file.txt", false)]
        [InlineData("", false)]
        public void IsInFolder_資料夾路徑中有沒有指定資料夾(string relativePath, bool expected)
        {
            // Arrange
            var stub = AppSettings.Empty();
            stub.IgnoreFolders = new[] { "bin", "obj" };
            var sut = new FolderReader(stub);

            // Act
            var result = sut.IsInFolder(relativePath, stub.IgnoreFolders);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("src\\bin\\debug", "bin", true)]
        [InlineData("src\\release", "bin", false)]
        [InlineData("src\\release\\bin.txt", "bin", false)]//忽略資料夾 檔名是關鍵字不會被忽略
        public void IsInDirectory_資料夾路徑中有沒有指定資料夾(string directoryName, string dir, bool expected)
        {
            // Arrange
            var stub = AppSettings.Empty();
            var sut = new FolderReader(stub);

            // Act
            var result = sut.IsInDirectory(directoryName, dir);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void CompareDictionary_比較兩個字典_回傳正確的FileStatus列表()
        {
            // Arrange
            var stub = AppSettings.Empty();
            var sut = new FolderReader(stub);

            var sourceDic = new Dictionary<string, string>
            {
                { "state\\add.txt", "C:\\source\\state\\add.txt" },
                { "state\\change.txt", "C:\\source\\state\\change.txt" }
            };

            var destDic = new Dictionary<string, string>
            {
                { "state\\change.txt", "C:\\dest\\state\\change.txt" },
                { "state\\delete.txt", "C:\\dest\\state\\delete.txt" }
            };

            // Act
            var result = sut.CompareDictionary(sourceDic, destDic);

            // Assert
            Assert.Equal(3, result.Count);
            var names = result.Select(x => x.檔名).ToList();
            Assert.Contains("add.txt", names);
            Assert.Contains("change.txt", names);
            Assert.Contains("delete.txt", names);
        }
    }
}
