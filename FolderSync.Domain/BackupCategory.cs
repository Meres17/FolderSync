namespace FolderSync.Domain
{
    /// <summary>
    /// 備份分類資料夾名稱，依檔案比對狀態歸檔，供更新與還原時對應。
    /// </summary>
    public static class BackupCategory
    {
        public const string Delete = "_state/Delete";
        public const string Diff = "_state/Diff";
        public const string Add = "_state/Add";
    }
}
