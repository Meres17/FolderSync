namespace FolderSync.Domain
{
    /// <summary>
    /// 掃描資料夾後的單一檔案快照，僅含比對所需的最小資訊。
    /// </summary>
    public class FileSnapshot
    {
        public FileSnapshot(string relativePath, string fullPath, DateTime lastWriteTime)
        {
            相對路徑 = relativePath;
            完整路徑 = fullPath;
            最後修改時間 = lastWriteTime;
        }

        public string 相對路徑 { get; }
        public string 完整路徑 { get; }
        public DateTime 最後修改時間 { get; }
    }
}
