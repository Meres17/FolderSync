
namespace FolderSyncCore
{
    public class FileStatus
    {
        public FileStatus(string path, string? sourcePath, string? destPath)
        {
            相對路徑 = path;
            檔名 = Path.GetFileName(path);
            來源時間 = GetLastWriteTime(sourcePath);
            目標時間 = GetLastWriteTime(destPath);
            來源路徑 = sourcePath;
            目標路徑 = destPath;
            狀態 = GetState(來源時間, 目標時間);
        }

        private DateTime? GetLastWriteTime(string? path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            return new FileInfo(path).LastWriteTime;
        }

        internal CompareState GetState(DateTime? sourceTime, DateTime? destTime)
        {
            return (sourceTime, destTime) switch
            {
                (null, null) => CompareState.不存在,
                (null, _) => CompareState.刪除檔案,
                (_, null) => CompareState.新增檔案,
                (_, _) => sourceTime == destTime ? CompareState.時間相同 : CompareState.時間不同,
            };
        }

        public CompareState 狀態 { get; }
        public string 檔名 { get; }
        public string 相對路徑 { get; }
        public DateTime? 來源時間 { get; }
        public string? 來源路徑 { get; }
        public DateTime? 目標時間 { get; }
        public string? 目標路徑 { get; }
    }
}
