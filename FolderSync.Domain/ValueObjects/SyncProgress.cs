namespace FolderSync.Domain.ValueObjects
{
    /// <summary>
    /// 同步進度：目前階段與已處理／總檔案數；總數為 0 表示該階段不計數（如等待站台離線）。
    /// </summary>
    public record SyncProgress(string 階段, int 已處理, int 總數)
    {
        public override string ToString()
        {
            return 總數 > 0 ? $"{階段} {已處理}/{總數}" : 階段;
        }
    }
}
