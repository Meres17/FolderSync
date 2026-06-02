
using NSubstitute;

namespace FolderSync.Tests.Domain.Services
{
    public class BackupPlanTests
    {
        [Fact]
        public void ForOverwrite_五項規則對應正確的資料夾與來源側()
        {
            // Act
            var rules = BackupPlan.ForOverwrite("src", "dst");

            // Assert
            Assert.Equal(5, rules.Count);
            Assert.Contains(rules, r => r.FolderName == "src" && r.Side == FileSide.來源);
            Assert.Contains(rules, r => r.FolderName == "dst" && r.Side == FileSide.目標);
            Assert.Contains(rules, r => r.FolderName == BackupCategory.Delete && r.Side == FileSide.目標);
            Assert.Contains(rules, r => r.FolderName == BackupCategory.Diff && r.Side == FileSide.目標);
            Assert.Contains(rules, r => r.FolderName == BackupCategory.Add && r.Side == FileSide.來源);
        }

        [Fact]
        public void Matches_依檔案狀態判斷()
        {
            // Arrange
            var rule = new BackupRule("x", FileSide.來源, new[] { CompareState.新增檔案 });

            // Act & Assert
            Assert.True(rule.Matches(FakeFile(CompareState.新增檔案)));
            Assert.False(rule.Matches(FakeFile(CompareState.刪除檔案)));
        }

        private static FileStatus FakeFile(CompareState state)
        {
            var file = Substitute.For<FileStatus>();
            file.狀態.Returns(state);
            return file;
        }
    }
}
