namespace FolderSyncCore
{
    public interface IAsyncFolderControl
    {
        Task OverwriteAsync(IEnumerable<FileStatus> files, string sourceDir, string destDir);
        Task RestoreAsync(string backupDir, string destDir);
    }
}
