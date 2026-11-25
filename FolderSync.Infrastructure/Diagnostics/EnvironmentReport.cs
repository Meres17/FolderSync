using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace FolderSync.Infrastructure.Diagnostics
{
    /// <summary>
    /// 每次啟動覆寫 environment.txt，記錄 OS 與已安裝的 .NET runtime，供日後更版判斷；寫入失敗不影響主流程。
    /// </summary>
    public class EnvironmentReport
    {
        private const string FileName = "environment.txt";
        private static readonly string[] Frameworks = { "Microsoft.NETCore.App", "Microsoft.AspNetCore.App", "Microsoft.WindowsDesktop.App" };
        private readonly string _path;
        private readonly TimeProvider _clock;

        public EnvironmentReport(TimeProvider clock, string? path = null)
        {
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _path = path ?? Path.Combine(Directory.GetCurrentDirectory(), FileName);
        }

        public void Write()
        {
            try
            {
                File.WriteAllText(_path, Build(), new UTF8Encoding(true));
            }
            catch
            {
                // 記錄失敗不影響主要流程
            }
        }

        private string Build()
        {
            var lines = new List<string>
            {
                $"產生時間：{_clock.GetLocalNow():yyyy-MM-dd HH:mm:ss}",
                $"OS：{RuntimeInformation.OSDescription} {RuntimeInformation.OSArchitecture}",
                $"工具：FolderSyncForm {Assembly.GetEntryAssembly()?.GetName().Version}，{RuntimeInformation.FrameworkDescription} {RuntimeInformation.ProcessArchitecture}",
            };
            foreach (var (label, root) in DotnetRoots())
            {
                lines.Add($"已安裝 runtime（{label}：{root}）：");
                lines.AddRange(ListRuntimes(root).Select(x => $"  {x}"));
            }

            return string.Join(Environment.NewLine, lines) + Environment.NewLine;
        }

        // 本工具是 x86 程序，ProgramFiles 會指到 (x86)；x64 安裝位置改讀 ProgramW6432，32 位元 OS 沒有此變數。
        private static IEnumerable<(string Label, string Root)> DotnetRoots()
        {
            var x64 = Environment.GetEnvironmentVariable("ProgramW6432");
            if (string.IsNullOrEmpty(x64) == false)
            {
                yield return ("x64", Path.Combine(x64, "dotnet"));
            }

            yield return ("x86", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "dotnet"));
        }

        /// <summary>
        /// 依 dotnet\shared\&lt;框架&gt;\&lt;版本&gt; 資料夾列出各框架已安裝版本（舊到新），沒有則標示未安裝。
        /// </summary>
        internal static IEnumerable<string> ListRuntimes(string dotnetRoot)
        {
            return Frameworks.Select(name => $"{name} {JoinVersions(Path.Combine(dotnetRoot, "shared", name))}");
        }

        private static string JoinVersions(string frameworkDir)
        {
            if (Directory.Exists(frameworkDir) == false)
                return "（未安裝）";

            var versions = Directory.GetDirectories(frameworkDir)
                .Select(Path.GetFileName)
                .OrderBy(ParseVersion)
                .ToList();
            return versions.Count == 0 ? "（未安裝）" : string.Join("、", versions);
        }

        // 資料夾名稱可能帶預覽標記（10.0.0-rc.1），只取前段比較，避免字串排序把 10 排在 8 前面。
        private static Version ParseVersion(string? name)
        {
            return Version.TryParse(name?.Split('-')[0], out var version) ? version : new Version();
        }
    }
}
