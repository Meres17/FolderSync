namespace FolderSync.Domain
{
    /// <summary>
    /// 單一備份規則：把符合指定狀態的檔案，依來源側備份到指定子資料夾。
    /// </summary>
    public sealed record BackupRule(string Folder, FileSide Side, IReadOnlyList<CompareState> States)
    {
        public bool Matches(FileStatus file)
        {
            return States.Contains(file.狀態);
        }
    }
}
