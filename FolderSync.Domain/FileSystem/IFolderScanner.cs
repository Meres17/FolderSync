namespace FolderSync.Domain.FileSystem
{
    /// <summary>
    /// 將資料夾掃描成檔案快照清單（不套用忽略規則）。
    /// </summary>
    public interface IFolderScanner
    {
        IReadOnlyList<FileSnapshot> Scan(string dir);
    }
}
