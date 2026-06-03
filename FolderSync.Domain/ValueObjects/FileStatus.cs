namespace FolderSync.Domain.ValueObjects
{
    public class FileStatus
    {
        internal FileStatus()
        {

        }
        public FileStatus(string path, string? sourcePath, DateTime? sourceTime, string? destPath, DateTime? destTime)
        {
            相對路徑 = path;
            檔名 = Path.GetFileName(path);
            來源路徑 = sourcePath;
            目標路徑 = destPath;
            來源時間 = sourceTime;
            目標時間 = destTime;
            狀態 = GetState(sourceTime, destTime);
        }

        internal static CompareState GetState(DateTime? sourceTime, DateTime? destTime)
        {
            return (sourceTime, destTime) switch
            {
                (null, null) => CompareState.不存在,
                (null, _) => CompareState.刪除檔案,
                (_, null) => CompareState.新增檔案,
                (_, _) => sourceTime == destTime ? CompareState.時間相同 : CompareState.時間不同,
            };
        }

        /// <summary>
        /// 取得指定側的實體路徑，供複製時選用來源或目標。
        /// </summary>
        public string? PathOf(FileSide side)
        {
            return side == FileSide.來源 ? 來源路徑 : 目標路徑;
        }

        public virtual CompareState 狀態 { get; }
        public string 檔名 { get; } = "";
        public virtual string 相對路徑 { get; } = "";
        public DateTime? 來源時間 { get; }
        public string? 來源路徑 { get; }
        public DateTime? 目標時間 { get; }
        public string? 目標路徑 { get; }
    }
}
