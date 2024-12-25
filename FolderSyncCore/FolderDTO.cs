namespace FolderSyncCore
{
    public class FolderDTO
    {
        public FolderDTO(string path)
        {
            完整路徑 = path;
            備份名稱 = Path.GetFileName(path);
        }

        public string 備份名稱 { get; }
        public string 完整路徑 { get; }
    }
}
