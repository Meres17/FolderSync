namespace FolderSync.Domain.FileSystem
{
    /// <summary>
    /// 檔案系統的複製與刪除操作（port）；目標相對路徑會接到指定的根目錄下。
    /// </summary>
    public interface IFileOperations
    {
        /// <param name="onFileDone">每處理完一個檔案呼叫一次（可並行呼叫），供回報進度。</param>
        Task CopyAsync(IEnumerable<CopyOperation> operations, string destinationRoot, Action? onFileDone = null);
        Task DeleteAsync(IEnumerable<string?> paths, Action? onFileDone = null);
    }
}
