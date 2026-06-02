
namespace FolderSync.Tests.Domain.Services
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

        [Fact]
        public void Compare_全部時間相同_回傳空清單()
        {
            // Arrange
            var source = new[] { Snap("a.txt", Older), Snap("b.txt", Older) };
            var dest = new[] { Snap("a.txt", Older), Snap("b.txt", Older) };

            // Act
            var result = new FolderComparer().Compare(source, dest, NoIgnore);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void Compare_依狀態再相對路徑排序()
        {
            // Arrange
            var source = new[]
            {
                Snap("z_add.txt", Newer),  // 新增
                Snap("a_add.txt", Newer),  // 新增
                Snap("diff.txt", Newer),   // 時間不同
            };
            var dest = new[]
            {
                Snap("diff.txt", Older),   // 時間不同
                Snap("del.txt", Older),    // 刪除
            };

            // Act
            var result = new FolderComparer().Compare(source, dest, NoIgnore);

            // Assert：先依狀態（新增 < 刪除 < 時間不同）排序
            var states = result.Select(x => x.狀態).ToList();
            Assert.Equal(states.OrderBy(x => x).ToList(), states);

            // 同狀態內依相對路徑排序
            var adds = result.Where(x => x.狀態 == CompareState.新增檔案).Select(x => x.檔名).ToList();
            Assert.Equal(new[] { "a_add.txt", "z_add.txt" }, adds);
        }

        private static FileSnapshot Snap(string relativePath, DateTime time)
        {
            return new FileSnapshot(relativePath, "full\\" + relativePath, time);
        }
    }
}
