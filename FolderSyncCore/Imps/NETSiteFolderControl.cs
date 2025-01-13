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

        public List<FolderDTO> GetFolders(string sourceDir, string destDir)
        {
            return _folderControl.GetFolders(sourceDir, destDir);
        }

        public void Overwrite(IEnumerable<FileStatus> files, string sourceDir, string destDir)
        {
            try
            {
                _siteControl.CloseSite(destDir);
                _folderControl.Overwrite(files, sourceDir, destDir);
            }
            finally
            {
                _siteControl.OpenSite(destDir);
            }
        }

        public void Restore(string backupDir, string destDir)
        {
            try
            {
                _siteControl.CloseSite(destDir);
                _folderControl.Restore(backupDir, destDir);
            }
            finally
            {
                _siteControl.OpenSite(destDir);
            }
        }
    }
}
