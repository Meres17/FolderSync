
namespace FolderSync.Tests.Domain.Services
{
    public class IgnorePolicyTests
    {
        [Theory]
        [InlineData("C:\\path\\to\\ignore.txt", true)]
        [InlineData("C:\\path\\to\\ignore.doc", true)]
        [InlineData("C:\\path\\to\\ignore.pdf", false)]//副檔名不是忽略清單
        [InlineData("C:\\path\\ignore.txt\\file.txt", false)]//路徑中有ignore.txt但檔名不是
        [InlineData("C:\\path\\to\\file.txt", false)]//完全不相同
        public void ShouldIgnore_依忽略檔名判斷(string relativePath, bool expected)
        {
            // Arrange
            var sut = new IgnorePolicy(new[] { "ignore.txt", "ignore.doc" }, Array.Empty<string>());

            // Act & Assert
            Assert.Equal(expected, sut.ShouldIgnore(relativePath));
        }

        [Theory]
        [InlineData("bin\\file.txt", true)]
        [InlineData("obj\\file.txt", true)]
        [InlineData("src\\bin\\file.txt", true)]
        [InlineData("src\\file.txt", false)]
        [InlineData("", false)]
        [InlineData("src\\release\\bin.txt", false)]//忽略資料夾的關鍵字當檔名不會被忽略
        public void ShouldIgnore_依忽略資料夾判斷(string relativePath, bool expected)
        {
            // Arrange
            var sut = new IgnorePolicy(Array.Empty<string>(), new[] { "bin", "obj" });

            // Act & Assert
            Assert.Equal(expected, sut.ShouldIgnore(relativePath));
        }
    }
}
