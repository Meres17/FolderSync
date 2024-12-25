
namespace FolderSyncCore
{
    public class FileDTO
    {
        public FileDTO(string path, FileInfo? source, FileInfo? target)
        {
            完整路徑 = source?.FullName ?? target?.FullName ?? path;
            相對路徑 = path;
            狀態 = GetState(source?.LastWriteTime, target?.LastWriteTime);
            檔名 = Path.GetFileName(path);
            來源時間 = source?.LastWriteTime;
            目標時間 = target?.LastWriteTime;
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
        public DateTime? 來源時間 { get; }
        public DateTime? 目標時間 { get; }
        public string 相對路徑 { get; }
        public string 完整路徑 { get; }
    }
}
