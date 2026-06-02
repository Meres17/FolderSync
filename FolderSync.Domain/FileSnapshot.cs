namespace FolderSync.Domain
{
    /// <summary>
    /// 掃描資料夾後的單一檔案快照，僅含比對所需的最小資訊。
    /// </summary>
    public record FileSnapshot(string 相對路徑, string 完整路徑, DateTime 最後修改時間);
}
