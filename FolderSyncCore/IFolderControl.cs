namespace FolderSyncCore
{
    public interface IFolderControl
    {
        void Overwrite(IEnumerable<FileStatus> files, string sourceDir, string destDir);
        void Restore(string backupDir, string destDir);
    }
}
