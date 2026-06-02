namespace FolderSync.Domain
{
    /// <summary>
    /// 將來源差異套用到目標（Apply），或從備份還原目標（Rollback）。
    /// </summary>
    public interface IFolderSynchronizer
    {
        Task ApplyAsync(IReadOnlyList<FileStatus> changes, string sourceDir, string destDir);
        Task RollbackAsync(string backupDir, string destDir);
    }
}
