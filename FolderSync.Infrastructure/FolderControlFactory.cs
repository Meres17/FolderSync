using FolderSync.Domain;

namespace FolderSync.Infrastructure
{
    public class FolderControlFactory : IFolderControlFactory
    {
        private readonly IFolderReader _reader;
        private readonly AppSettings _appSettings;

        public FolderControlFactory(IFolderReader reader, AppSettings appSettings)
        {
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        }

        public string[] GetNames()
        {
            return new[] { ".NET站台", "資料夾" };
        }

        public IFolderControl CreateControl(string name)
        {
            return name switch
            {
                "資料夾" => new FolderControl(_reader),
                ".NET站台" => new NETSiteFolderControl(new FolderControl(_reader), new SiteControl(), _appSettings),
                _ => throw new NotSupportedException($"不支援的備份類型：{name}")
            };
        }

        public IAsyncFolderControl CreateAsyncControl(string name)
        {
            return name switch
            {
                "資料夾" => new AsyncFolderControl(_reader),
                ".NET站台" => new AsyncNETSiteFolderControl(new AsyncFolderControl(_reader), new SiteControl(), _appSettings),
                _ => throw new NotSupportedException($"不支援的備份類型：{name}")
            };
        }
    }
}
