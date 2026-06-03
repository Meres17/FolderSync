namespace FolderSync.Infrastructure.Synchronizers
{
    public class FolderSynchronizerFactory : IFolderSynchronizerFactory
    {
        private readonly IBackupRepository _backups;
        private readonly AppSettingsProvider _settings;
        private readonly IFileOps _files;

        public FolderSynchronizerFactory(IBackupRepository backups, AppSettingsProvider settings, IFileOps files)
        {
            _backups = backups ?? throw new ArgumentNullException(nameof(backups));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _files = files ?? throw new ArgumentNullException(nameof(files));
        }

        public IReadOnlyList<SyncMode> Modes => Enum.GetValues<SyncMode>();

        public IFolderSynchronizer Create(SyncMode mode)
        {
            return mode switch
            {
                SyncMode.資料夾 => new FolderSynchronizer(_backups, _files),
                SyncMode.站台 => new SiteFolderSynchronizer(new FolderSynchronizer(_backups, _files), new SiteController(), _settings.Current.SiteDelay),
                _ => throw new NotSupportedException($"不支援的同步模式：{mode}")
            };
        }
    }
}
