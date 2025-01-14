namespace FolderSyncCore.Imps
{
    internal class NETSiteFolderControl : IFolderControl
    {
        private readonly IFolderControl _folderControl;
        private readonly ISiteControl _siteControl;

        public NETSiteFolderControl(IFolderControl folderControl, ISiteControl siteControl)
        {
            _folderControl = folderControl ?? throw new ArgumentNullException(nameof(folderControl));
            _siteControl = siteControl ?? throw new ArgumentNullException(nameof(siteControl));
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
                action();
            }
            finally
            {
                _siteControl.OpenSite(destDir);
            }
        }
    }
}
