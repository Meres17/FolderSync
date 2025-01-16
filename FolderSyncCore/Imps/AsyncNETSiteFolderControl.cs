namespace FolderSyncCore.Imps
{
    internal class AsyncNETSiteFolderControl : IAsyncFolderControl
    {
        private readonly IAsyncFolderControl _folderControl;
        private readonly ISiteControl _siteControl;
        private readonly AppSettings _appSettings;

        public AsyncNETSiteFolderControl(IAsyncFolderControl folderControl, ISiteControl siteControl, AppSettings appSettings)
        {
            _folderControl = folderControl ?? throw new ArgumentNullException(nameof(folderControl));
            _siteControl = siteControl ?? throw new ArgumentNullException(nameof(siteControl));
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        }
        public Task OverwriteAsync(IEnumerable<FileStatus> files, string sourceDir, string destDir)
        {
            return ExecuteWithSiteControl(destDir, () => _folderControl.OverwriteAsync(files, sourceDir, destDir));
        }

        public Task RestoreAsync(string backupDir, string destDir)
        {
            return ExecuteWithSiteControl(destDir, () => _folderControl.RestoreAsync(backupDir, destDir));
        }

        internal virtual async Task ExecuteWithSiteControl(string destDir, Func<Task> action)
        {
            try
            {
                _siteControl.CloseSite(destDir);
                await Task.Delay(_appSettings.SiteDelay);
                await action();
            }
            finally
            {
                _siteControl.OpenSite(destDir);
            }
        }
    }
}
