namespace FolderSync.Domain.Services
{
    /// <summary>
    /// 掃描來源與目標資料夾並比對，產出差異清單的領域服務。
    /// </summary>
    public class FolderDiffService
    {
        private readonly IFolderScanner _scanner;
        private readonly FolderComparer _comparer;
        private readonly AppSettingsProvider _settings;

        public FolderDiffService(IFolderScanner scanner, FolderComparer comparer, AppSettingsProvider settings)
        {
            _scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));
            _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public List<FileStatus> GetDiff(string sourceDir, string destDir)
        {
            var source = _scanner.Scan(sourceDir);
            var dest = _scanner.Scan(destDir);
            var ignore = IgnorePolicy.From(_settings.Current);
            return _comparer.Compare(source, dest, ignore);
        }

        /// <summary>
        /// 只掃描一次，同時產出要同步的變更與因忽略規則而略過的差異。
        /// </summary>
        public UpdatePlan GetPlan(string sourceDir, string destDir)
        {
            var source = _scanner.Scan(sourceDir);
            var dest = _scanner.Scan(destDir);
            var ignore = IgnorePolicy.From(_settings.Current);
            return new UpdatePlan(
                sourceDir,
                destDir,
                _comparer.Compare(source, dest, ignore),
                _comparer.CompareIgnored(source, dest, ignore));
        }
    }
}
