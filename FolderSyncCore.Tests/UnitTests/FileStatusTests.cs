namespace FolderSyncCore.Tests.UnitTests
{
    public class FileStatusTests
    {
        [Theory]
        [InlineData(null, null, CompareState.不存在)]
        [InlineData(null, "2023-10-01", CompareState.刪除檔案)]
        [InlineData("2023-10-01", null, CompareState.新增檔案)]
        [InlineData("2023-10-01", "2023-10-01", CompareState.時間相同)]
        [InlineData("2023-10-01", "2023-10-02", CompareState.時間不同)]
        public void GetState_測試不同的時間組合(string? sourceTimeStr, string? destTimeStr, CompareState expect)
        {
            // Arrange

            DateTime? sourceTime = ToDateTime(sourceTimeStr);
            DateTime? destTime = ToDateTime(destTimeStr);

            var sut = new FileStatus("test.txt", "source/test.txt", "dest/test.txt");

            // Act
            var result = sut.GetState(sourceTime, destTime);

            // Assert
            Assert.Equal(expect, result);
        }

        private static DateTime? ToDateTime(string? sourceTimeStr)
        {
            return string.IsNullOrEmpty(sourceTimeStr)
                ? null
                : DateTime.Parse(sourceTimeStr);
        }
    }
}
