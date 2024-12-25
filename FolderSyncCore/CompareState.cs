namespace FolderSyncCore
{
    public enum CompareState
    {
        不存在 = 0,
        新增檔案 = 1,
        刪除檔案 = 2,
        時間相同 = 3,
        時間不同 = 4
    }
}
