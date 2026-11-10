
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
        [InlineData("appsettings.json", true)]
        [InlineData("appsettings.Production.json", true)]
        [InlineData("sub\\appsettings.Staging.json", true)]
        [InlineData("APPSETTINGS.JSON", true)]//不分大小寫
        [InlineData("IMO.pdb", true)]
        [InlineData("sub\\IMO.PDB", true)]
        [InlineData("myappsettings.json", false)]//星號前的字首不符
        [InlineData("appsettings.txt", false)]//副檔名不符
        [InlineData("IMO.pdb.txt", false)]//pdb 不在結尾
        [InlineData("appsettings.json\\a.txt", false)]//符合的是資料夾名稱，不是檔名
        public void ShouldIgnore_忽略清單含萬用字元_依完整檔名判斷(string relativePath, bool expected)
        {
            // arrange
            var sut = new IgnorePolicy(new[] { "appsettings*.json", "*.pdb" }, Array.Empty<string>());

            // act
            var result = sut.ShouldIgnore(relativePath);

            // assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("web.config", true)]
        [InlineData("Views\\Web.Config", true)]
        [InlineData("xweb.config", false)]//只有結尾相同不算符合
        public void ShouldIgnore_忽略清單為一般檔名_需完整檔名相符(string relativePath, bool expected)
        {
            // arrange
            var sut = new IgnorePolicy(new[] { "web.config" }, Array.Empty<string>());

            // act
            var result = sut.ShouldIgnore(relativePath);

            // assert
            Assert.Equal(expected, result);
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
