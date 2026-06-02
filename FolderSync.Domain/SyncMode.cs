namespace FolderSync.Domain
{
    /// <summary>
    /// 同步模式：純資料夾，或先讓 .NET 站台離線再操作。
    /// </summary>
    public enum SyncMode
    {
        資料夾 = 0,
        站台 = 1
    }
}
