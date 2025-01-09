namespace FolderSyncCore
{
    public interface IBackup
    {
        List<FolderDTO> GetFolders(string sourceDir, string destDir);
        void Backup(IEnumerable<FileStatus> files, string sourceDir, string destDir);
        void Restore(string backupDir, string destDir);
    }
}
