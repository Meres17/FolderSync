using FolderSyncCore.Imps;

namespace FolderSyncCore
{
    public class BackupFactory
    {
        private readonly IFolderReader _reader;

        public BackupFactory(AppSettings appSettings)
        {
            _reader = new FolderReader(appSettings);
        }

        public string[] GetNames()
        {
            return new[] { "資料夾", ".NET站台" };
        }

        public IBackup Create(string name)
        {
            return name switch
            {
                "資料夾" => new FolderBackup(_reader),
                ".NET站台" => new NetSiteBackup(new FolderBackup(_reader)),
                _ => throw new NotSupportedException($"不支援的備份類型：{name}")
            };
        }
    }
}
