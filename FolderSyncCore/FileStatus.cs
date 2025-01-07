
namespace FolderSyncCore
{
    public class FileStatus
    {
        public FileStatus(string path, FileInfo? source, FileInfo? target)
        {
            相對路徑 = path;
            狀態 = GetState(source?.LastWriteTime, target?.LastWriteTime);
            檔名 = Path.GetFileName(path);
            來源時間 = source?.LastWriteTime;
            來源路徑 = source?.FullName;
            目標時間 = target?.LastWriteTime;
            目標路徑 = target?.FullName;
        }

        private CompareState GetState(DateTime? sourceTime, DateTime? targetTime)
        {
            return (sourceTime, targetTime) switch
            {
                (null, null) => CompareState.不存在,
                (null, _) => CompareState.刪除檔案,
                (_, null) => CompareState.新增檔案,
                (_, _) => sourceTime == targetTime ? CompareState.時間相同 : CompareState.時間不同,
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
