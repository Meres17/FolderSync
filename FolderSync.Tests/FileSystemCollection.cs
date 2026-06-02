namespace FolderSync.Tests
{
    /// <summary>
    /// 會切換目前工作目錄的測試共用此 collection，確保彼此循序執行、不互相干擾。
    /// </summary>
    [CollectionDefinition("FileSystem")]
    public class FileSystemCollection
    {
    }
}
