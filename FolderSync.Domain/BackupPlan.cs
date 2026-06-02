namespace FolderSync.Domain
{
    /// <summary>
    /// 更新前的備份計畫：定義各狀態的檔案要備份到哪些資料夾，作為還原依據。
    /// </summary>
    public static class BackupPlan
    {
        public static IReadOnlyList<BackupRule> ForOverwrite(string sourceFolderName, string destFolderName)
        {
            return new[]
            {
                new BackupRule(sourceFolderName, FileSide.來源, new[] { CompareState.新增檔案, CompareState.時間不同 }),
                new BackupRule(destFolderName, FileSide.目標, new[] { CompareState.刪除檔案, CompareState.時間不同 }),
                new BackupRule(BackupCategory.Delete, FileSide.目標, new[] { CompareState.刪除檔案 }),
                new BackupRule(BackupCategory.Diff, FileSide.目標, new[] { CompareState.時間不同 }),
                new BackupRule(BackupCategory.Add, FileSide.來源, new[] { CompareState.新增檔案 }),
            };
        }
    }
}
