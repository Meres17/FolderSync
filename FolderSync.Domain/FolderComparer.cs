namespace FolderSync.Domain
{
    /// <summary>
    /// 比對來源與目標的檔案快照，產出有差異的檔案狀態清單。
    /// </summary>
    public class FolderComparer
    {
        public List<FileStatus> Compare(
            IEnumerable<FileSnapshot> source,
            IEnumerable<FileSnapshot> dest,
            IgnorePolicy ignore)
        {
            var sourceMap = ToMap(source, ignore);
            var destMap = ToMap(dest, ignore);

            var result = new List<FileStatus>();
            foreach (var (relativePath, sourceFile) in sourceMap)
            {
                destMap.Remove(relativePath, out var destFile);
                result.Add(Build(relativePath, sourceFile, destFile));
            }

            // 剩餘的 destMap 即為僅存在於目標的檔案
            foreach (var (relativePath, destFile) in destMap)
            {
                result.Add(Build(relativePath, null, destFile));
            }

            return result
                .Where(x => x.狀態 != CompareState.時間相同)
                .OrderBy(x => x.狀態)
                .ThenBy(x => x.相對路徑)
                .ToList();
        }

        private static Dictionary<string, FileSnapshot> ToMap(IEnumerable<FileSnapshot> snapshots, IgnorePolicy ignore)
        {
            return snapshots
                .Where(x => !ignore.ShouldIgnore(x.相對路徑))
                .ToDictionary(x => x.相對路徑);
        }

        private static FileStatus Build(string relativePath, FileSnapshot? source, FileSnapshot? dest)
        {
            return new FileStatus(
                relativePath,
                source?.完整路徑, source?.最後修改時間,
                dest?.完整路徑, dest?.最後修改時間);
        }
    }
}
