using FolderSyncCore.Imps;

namespace FolderSyncCore
{
    public class FolderFactory
    {
        private readonly IFolderReader _reader;

        public FolderFactory(AppSettings appSettings)
        {
            _reader = new FolderReader(appSettings);
        }

        public string[] GetNames()
        {
            return new[] { ".NET站台", "資料夾" };
        }

        public IFolderReader GetReader()
        {
            return _reader;
        }

        public IFolderControl CreateControl(string name)
        {
            return name switch
            {
                "資料夾" => new FolderControl(_reader),
                ".NET站台" => new NETSiteFolderControl(new FolderControl(_reader)),
                _ => throw new NotSupportedException($"不支援的備份類型：{name}")
            };
        }
    }
}
