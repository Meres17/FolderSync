namespace FolderSync.Domain.Interfaces
{
    /// <summary>
    /// 檔案系統的複製與刪除操作（port）；目標相對路徑會接到指定的根目錄下。
    /// </summary>
    public interface IFileOps
    {
        Task CopyAsync(IEnumerable<CopyOperation> operations, string destinationRoot);
        Task DeleteAsync(IEnumerable<string?> paths);
    }
}
