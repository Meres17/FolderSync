
namespace FolderSync.Infrastructure.Scanning
{
    public class FolderScanner : IFolderScanner
    {
        public IReadOnlyList<FileSnapshot> Scan(string dir)
        {
            if (string.IsNullOrEmpty(dir))
            {
                throw new DomainException("請輸入資料夾路徑");
            }

            if (!Directory.Exists(dir))
            {
                throw new DomainException($"找不到資料夾：{dir}");
            }

            // 直接列舉 FileInfo，沿用列舉時已取得的時間戳，省去逐檔再 stat 一次。
            return new DirectoryInfo(dir)
                .EnumerateFiles("*", SearchOption.AllDirectories)
                .Select(file => new FileSnapshot(Path.GetRelativePath(dir, file.FullName), file.FullName, file.LastWriteTime))
                .ToList();
        }
    }
}
