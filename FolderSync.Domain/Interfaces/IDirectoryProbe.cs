namespace FolderSync.Domain.Interfaces
{
    /// <summary>
    /// 查詢資料夾是否存在（檔案系統存取的 port）。
    /// </summary>
    public interface IDirectoryProbe
    {
        bool Exists(string path);
    }
}
