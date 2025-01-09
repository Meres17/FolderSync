namespace FolderSyncCore
{
    public interface IFolderReader
    {
        Dictionary<string, string> GetPathDictionary(string dir);
        List<FileStatus> GetDiffFiles(string sourceDir, string destDir);
    }
}
