namespace FolderSync.Domain.Repositories
{
    /// <summary>
    /// 備份的讀寫倉儲：建立備份目錄、列出與刪除備份、讀取備份內容供還原。
    /// </summary>
    public interface IBackupRepository
    {
        string CreateBackupDirectory(string sourceDir, string destDir);
        IReadOnlyList<BackupFolder> ListBackups(string sourceDir, string destDir);
        IReadOnlyList<FileSnapshot> ReadBackup(string backupDir, string category);
        void DeleteBackup(string fullPath);
    }
}
