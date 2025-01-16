namespace FolderSyncCore.Imps
{
    internal class NETSiteFolderControl : IFolderControl
    {
        private readonly IFolderControl _folderControl;
        private readonly ISiteControl _siteControl;
        private readonly AppSettings _appSettings;

        public NETSiteFolderControl(IFolderControl folderControl, ISiteControl siteControl, AppSettings appSettings)
        {
            _folderControl = folderControl ?? throw new ArgumentNullException(nameof(folderControl));
            _siteControl = siteControl ?? throw new ArgumentNullException(nameof(siteControl));
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        }

        public void Overwrite(IEnumerable<FileStatus> files, string sourceDir, string destDir)
        {
            ExecuteWithSiteControl(destDir, () => _folderControl.Overwrite(files, sourceDir, destDir));
        }

        public void Restore(string backupDir, string destDir)
        {
            ExecuteWithSiteControl(destDir, () => _folderControl.Restore(backupDir, destDir));
        }

        internal virtual void ExecuteWithSiteControl(string destDir, Action action)
        {
            try
            {
                _siteControl.CloseSite(destDir);
                Thread.Sleep(_appSettings.SiteDelay);
                action();
            }
            finally
            {
                _siteControl.OpenSite(destDir);
            }
        }
    }
}
