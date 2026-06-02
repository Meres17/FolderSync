namespace FolderSync.Domain
{
    public interface IFolderReader
    {
        List<FileStatus> GetDiffFiles(string sourceDir, string destDir);
        List<FileStatus> GetBackupFiles(string host, string name);
        List<BackupFolder> GetBackupFolders(string sourceDir, string destDir);
        string CreateBackupDirectory(string sourceDir, string destDir);
        void DeleteBackupFolder(string fullPath);
    }
}
