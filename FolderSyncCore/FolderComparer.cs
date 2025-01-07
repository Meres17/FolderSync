namespace FolderSyncCore
{
    public class FolderComparer
    {
        private readonly string _sourceDir;
        private readonly string _targetDir;
        private static AppSettings _appSettings = AppSettings.Build();

        public FolderComparer(string sourceFolder, string targetFolder)
        {
            _sourceDir = sourceFolder;
            _targetDir = targetFolder;
        }

        public List<FileStatus> GetDiffFiles()
        {
            return new DictionaryComparer(_appSettings).GetFiles(_sourceDir, _targetDir);
        }

        public void Backup()
        {
            var files = GetDiffFiles();
            var filebackup = new FileBackup(_appSettings);
            filebackup.Backup(files, _sourceDir, _targetDir);
        }

        public List<FolderDTO> GetBackupFolders()
        {
            var filebackup = new FileBackup(_appSettings);
            return filebackup.GetFolders(_sourceDir, _targetDir);
        }

        public void Restore(string backupDir)
        {
            new FileBackup(_appSettings).Restore(backupDir, _targetDir);
        }

        public bool ExistOffline()
        {
            return new Site().CanClose();
        }

        public void CloseSite()
        {
            new Site().CloseSite(_targetDir);
        }

        public void OpenSite()
        {
            new Site().OpenSite(_targetDir);
        }
    }
}
