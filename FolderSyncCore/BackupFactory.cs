using FolderSyncCore.Imps;

namespace FolderSyncCore
{
    public class BackupFactory
    {
        private readonly AppSettings _appSettings;

        public BackupFactory(AppSettings appSettings)
        {
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        }

        public string[] GetNames()
        {
            return new[] { "資料夾", ".NET站台" };
        }

        public IBackup Create(string name)
        {
            return name switch
            {
                "資料夾" => new FolderBackup(_appSettings),
                ".NET站台" => new NetSiteBackup(new FolderBackup(_appSettings)),
                _ => throw new NotSupportedException($"不支援的備份類型：{name}")
            };
        }
    }
}
