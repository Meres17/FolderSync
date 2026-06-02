namespace FolderSync.Domain.Interfaces
{
    /// <summary>
    /// 操作稽核日誌：記錄更新／還原／刪除等行為與錯誤。
    /// </summary>
    public interface IActivityLog
    {
        void Info(string message);
        void Error(string message, Exception exception);
    }
}
