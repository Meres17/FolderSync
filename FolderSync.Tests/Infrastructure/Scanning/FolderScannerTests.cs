
namespace FolderSync.Tests.Infrastructure.Scanning
{
    public class FolderScannerTests
    {
        [Fact]
        public void Scan_找不到資料夾_拋出DomainException()
        {
            Assert.Throws<DomainException>(() => new FolderScanner().Scan("anypath"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Scan_空白路徑_拋出DomainException(string dir)
        {
            Assert.Throws<DomainException>(() => new FolderScanner().Scan(dir));
        }
    }
}
