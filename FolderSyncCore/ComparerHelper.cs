using System.Globalization;

namespace FolderSyncCore
{
    internal class ComparerHelper
    {
        private const string Format = "yyyyMMdd_HHmm";

        public static string CreateBackupDirectory(string sourceDir, string targetDir)
        {
            var host = CreateBackupHost(sourceDir, targetDir);
            var time = DateTime.Now.ToString(Format);
            return Path.Combine(host, time);
        }

        private static string CreateBackupHost(string sourceDir, string targetDir)
        {
            var host = Directory.GetCurrentDirectory();
            var sourceFileName = Path.GetFileName(sourceDir);
            var targetFileName = Path.GetFileName(targetDir);
            var result = Path.Combine(host, "Backup", $"{sourceFileName}_{targetFileName}");
            CreateDirectory(result);
            return result;
        }

        private static void CreateDirectory(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        public static void Copy(IEnumerable<FileDTO> dtos, string folder, Func<FileDTO, string> func)
        {
            foreach (var dto in dtos)
            {
                var sourcePath = func.Invoke(dto);
                var targetPath = Path.Combine(folder, dto.相對路徑);
                var targetFolder = Path.GetDirectoryName(targetPath);
                CreateDirectory(targetFolder);
                File.Copy(sourcePath, targetPath, true);
            }
        }

        public static void Delete(IEnumerable<FileDTO> dtos, Func<FileDTO, string> func)
        {
            foreach (var dto in dtos)
            {
                var path = func.Invoke(dto);
                File.Delete(path);
            }
        }


        public static List<FolderDTO> GetBackupFolders(string sourceDir, string targetDir)
        {
            var backupHost = CreateBackupHost(sourceDir, targetDir);
            return Directory
                .EnumerateDirectories(backupHost, "*", SearchOption.TopDirectoryOnly)
                .Where(x => DateTime.TryParseExact(Path.GetFileName(x), Format, null, DateTimeStyles.None, out _))
                .Select(x => new FolderDTO(x))
                .ToList();
        }

        public static Dictionary<string, string> GetPathDictionary(string source, IEnumerable<string> ignoreFiles, IEnumerable<string> ignoreFolders)
        {
            return Directory
                .EnumerateFiles(source, "*", SearchOption.AllDirectories)
                .Select(path => new
                {
                    Path = path,
                    RelativePath = Path.GetRelativePath(source, path)
                })
                .ToList()
                .Where(x => !IsIgnoreFile(x.Path, ignoreFiles.ToArray()))
                .Where(x => !IsInFolder(x.RelativePath, ignoreFolders.ToArray()))
                .ToDictionary(x => x.RelativePath, x => x.Path);
        }

        private static bool IsIgnoreFile(string path, params string[] excludedFiles)
        {
            return excludedFiles.Any(excludedFile => path.EndsWith(excludedFile, StringComparison.InvariantCultureIgnoreCase));
        }

        private static bool IsInFolder(string relativePath, params string[] dirs)
        {
            var directoryName = Path.GetDirectoryName(relativePath);
            if (string.IsNullOrEmpty(directoryName))
            {
                return false;
            }
            return dirs.Any(dir => IsInDirectory(directoryName, dir));
        }

        private static bool IsInDirectory(string directoryName, string dir)
        {
            return directoryName
                .Split(Path.DirectorySeparatorChar)
                .Contains(dir, StringComparer.InvariantCultureIgnoreCase);
        }

    }
}
