namespace FolderSync.Infrastructure.Logging
{
    /// <summary>
    /// 將操作日誌附加寫入檔案；記錄失敗絕不影響主流程。
    /// </summary>
    public class FileActivityLog : IActivityLog
    {
        private static readonly object Gate = new();
        private readonly string _path;

        public FileActivityLog(string? path = null)
        {
            _path = path ?? Path.Combine(Directory.GetCurrentDirectory(), "logs", "foldersync.log");
        }

        public void Info(string message)
        {
            Write("INFO", message);
        }

        public void Error(string message, Exception exception)
        {
            Write("ERROR", $"{message}{Environment.NewLine}{exception}");
        }

        private void Write(string level, string message)
        {
            try
            {
                lock (Gate)
                {
                    var dir = Path.GetDirectoryName(_path);
                    if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    File.AppendAllText(_path, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}{Environment.NewLine}");
                }
            }
            catch
            {
                // 記錄失敗不影響主要流程
            }
        }
    }
}
