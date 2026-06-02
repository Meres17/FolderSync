
namespace FolderSync.Tests.Infrastructure.Scanning
{
    public class FolderScannerTests
    {
        [Fact]
        public void Scan_無效路徑_拋出DirectoryNotFoundException()
        {
            Assert.Throws<DirectoryNotFoundException>(() => new FolderScanner().Scan("anypath"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Scan_空白路徑_拋出ArgumentException(string dir)
        {
            Assert.Throws<ArgumentException>(() => new FolderScanner().Scan(dir));
        }
    }
}
