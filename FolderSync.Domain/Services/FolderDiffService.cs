namespace FolderSync.Domain.Services
{
    /// <summary>
    /// 掃描來源與目標資料夾並比對，產出差異清單的領域服務。
    /// </summary>
    public class FolderDiffService
    {
        private readonly IFolderScanner _scanner;
        private readonly FolderComparer _comparer;
        private readonly IgnorePolicy _ignore;

        public FolderDiffService(IFolderScanner scanner, FolderComparer comparer, IgnorePolicy ignore)
        {
            _scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));
            _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            _ignore = ignore ?? throw new ArgumentNullException(nameof(ignore));
        }

        public List<FileStatus> GetDiff(string sourceDir, string destDir)
        {
            var source = _scanner.Scan(sourceDir);
            var dest = _scanner.Scan(destDir);
            return _comparer.Compare(source, dest, _ignore);
        }
    }
}
