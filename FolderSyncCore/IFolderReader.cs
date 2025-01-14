namespace FolderSyncCore
{
    public interface IFolderReader
    {
        Dictionary<string, string> GetPathDictionary(string dir);
        List<FileStatus> GetDiffFiles(string sourceDir, string destDir);
        List<FileStatus> GetRestoreFiles(string host, string name);
        List<FolderDTO> GetFolders(string sourceDir, string destDir);
        string CreateBackupDirectory(string sourceDir, string destDir);
    }
}
