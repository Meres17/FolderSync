namespace FolderSyncCore
{
    public class FolderService
    {
        private readonly AppSettings _appSettings;
        private readonly FileBackup _filebackup;
        private readonly DictionaryComparer _dictionaryComparer;


        public FolderService(AppSettings appSettings)
        {
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            _filebackup = new FileBackup(appSettings);
            _dictionaryComparer = new DictionaryComparer(appSettings);
        }

        public void Backup(string sourceDir, string destDir)
        {
            var files = GetDiffFiles(sourceDir, destDir);
            _filebackup.Backup(files, sourceDir, destDir);
        }

        public List<FileStatus> GetDiffFiles(string sourceDir, string destDir)
        {
            return _dictionaryComparer.GetFiles(sourceDir, destDir);
        }

        public List<FolderDTO> GetBackupFolders(string sourceDir, string destDir)
        {
            return _filebackup.GetFolders(sourceDir, destDir);
        }

        public void Restore(string backupDir, string destDir)
        {
            _filebackup.Restore(backupDir, destDir);
        }
    }
}
