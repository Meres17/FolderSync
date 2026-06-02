using FolderSync.Domain;

namespace FolderSync.Tests.Domain
{
    public class FolderComparerTests
    {
        private static readonly IgnorePolicy NoIgnore = new(Array.Empty<string>(), Array.Empty<string>());
        private static readonly DateTime Older = new(2023, 1, 1);
        private static readonly DateTime Newer = new(2023, 6, 1);

        [Fact]
        public void Compare_回傳新增刪除時間不同並排除時間相同()
        {
            // Arrange
            var source = new[]
            {
                Snap("add.txt", Newer),
                Snap("diff.txt", Newer),
                Snap("same.txt", Older),
            };
            var dest = new[]
            {
                Snap("diff.txt", Older),
                Snap("same.txt", Older),
                Snap("delete.txt", Older),
            };

            // Act
            var result = new FolderComparer().Compare(source, dest, NoIgnore);

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Contains(result, x => x.檔名 == "add.txt" && x.狀態 == CompareState.新增檔案);
            Assert.Contains(result, x => x.檔名 == "diff.txt" && x.狀態 == CompareState.時間不同);
            Assert.Contains(result, x => x.檔名 == "delete.txt" && x.狀態 == CompareState.刪除檔案);
            Assert.DoesNotContain(result, x => x.檔名 == "same.txt");
        }

        [Fact]
        public void Compare_套用忽略規則排除檔案()
        {
            // Arrange
            var ignore = new IgnorePolicy(new[] { "secret.txt" }, Array.Empty<string>());
            var source = new[] { Snap("secret.txt", Newer), Snap("keep.txt", Newer) };
            var dest = Array.Empty<FileSnapshot>();

            // Act
            var result = new FolderComparer().Compare(source, dest, ignore);

            // Assert
            Assert.Single(result);
            Assert.Equal("keep.txt", result[0].檔名);
        }

        private static FileSnapshot Snap(string relativePath, DateTime time)
        {
            return new FileSnapshot(relativePath, "full\\" + relativePath, time);
        }
    }
}
