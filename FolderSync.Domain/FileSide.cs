namespace FolderSync.Domain
{
    /// <summary>
    /// 檔案的來源側，用來決定複製時取用來源或目標的實體路徑。
    /// </summary>
    public enum FileSide
    {
        來源 = 0,
        目標 = 1
    }
}
