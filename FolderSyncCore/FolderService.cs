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

        public void Backup(string sourceDir, string targetDir)
        {
            var files = GetDiffFiles(sourceDir, targetDir);
            _filebackup.Backup(files, sourceDir, targetDir);
        }

        public List<FileStatus> GetDiffFiles(string sourceDir, string targetDir)
        {
            return _dictionaryComparer.GetFiles(sourceDir, targetDir);
        }

        public List<FolderDTO> GetBackupFolders(string sourceDir, string targetDir)
        {
            return _filebackup.GetFolders(sourceDir, targetDir);
        }

        public void Restore(string backupDir, string targetDir)
        {
            _filebackup.Restore(backupDir, targetDir);
        }
    }
}
