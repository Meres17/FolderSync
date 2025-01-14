namespace FolderSyncCore
{
    public interface IFolderReader
    {
        List<FileStatus> GetDiffFiles(string sourceDir, string destDir);
        List<FileStatus> GetBackupFiles(string host, string name);
        List<FolderDTO> GetBackupFolders(string sourceDir, string destDir);
        string CreateBackupDirectory(string sourceDir, string destDir);
    }
}
