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

        public List<FileStatus> GetDiffFiles(string sourceDir, string destDir)
        {
            return _reader.GetDiffFiles(sourceDir, destDir);
        }

        public List<FolderDTO> GetBackupFolders(string sourceDir, string destDir)
        {
            return _reader.GetBackupFolders(sourceDir, destDir);
        }

        public IFolderControl CreateControl(string name)
        {
            return name switch
            {
                "資料夾" => new FolderControl(_reader),
                ".NET站台" => new NETSiteFolderControl(new FolderControl(_reader), new SiteControl()),
                _ => throw new NotSupportedException($"不支援的備份類型：{name}")
            };
        }

        public IAsyncFolderControl CreateAsyncControl(string name)
        {
            return name switch
            {
                "資料夾" => new AsyncFolderControl(_reader),
                ".NET站台" => new AsyncNETSiteFolderControl(new AsyncFolderControl(_reader), new SiteControl()),
                _ => throw new NotSupportedException($"不支援的備份類型：{name}")
            };
        }
    }
}
