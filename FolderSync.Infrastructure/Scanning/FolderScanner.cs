
namespace FolderSync.Infrastructure.Scanning
{
    public class FolderScanner : IFolderScanner
    {
        public IReadOnlyList<FileSnapshot> Scan(string dir)
        {
            if (string.IsNullOrEmpty(dir))
            {
                throw new ArgumentException("請輸入資料夾路徑");
            }

            if (!Directory.Exists(dir))
            {
                throw new DirectoryNotFoundException($"找不到資料夾：{dir}");
            }

            return Directory
                .EnumerateFiles(dir, "*", SearchOption.AllDirectories)
                .Select(path => new FileSnapshot(Path.GetRelativePath(dir, path), path, new FileInfo(path).LastWriteTime))
                .ToList();
        }
    }
}
