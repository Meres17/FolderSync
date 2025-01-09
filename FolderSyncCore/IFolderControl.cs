namespace FolderSyncCore
{
    public interface IFolderControl
    {
        List<FolderDTO> GetFolders(string sourceDir, string destDir);
        void Overwrite(IEnumerable<FileStatus> files, string sourceDir, string destDir);
        void Restore(string backupDir, string destDir);
    }
}
