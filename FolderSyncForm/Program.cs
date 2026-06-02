using FolderSync.Application;
using FolderSync.Domain;
using FolderSync.Infrastructure;

using Microsoft.Extensions.DependencyInjection;

namespace FolderSyncForm
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            using var provider = BuildServiceProvider();
            Application.Run(provider.GetRequiredService<FolderSyncForm>());
        }

        private static ServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IAppSettingsRepository, JsonAppSettingsRepository>();
            services.AddTransient(sp => sp.GetRequiredService<IAppSettingsRepository>().Load());
            services.AddTransient(sp => IgnorePolicy.From(sp.GetRequiredService<AppSettings>()));
            services.AddSingleton<FolderComparer>();
            services.AddSingleton<IFolderScanner, FolderScanner>();
            services.AddTransient<IBackupRepository, FileSystemBackupRepository>();
            services.AddTransient<IFolderSynchronizerFactory, FolderSynchronizerFactory>();
            services.AddTransient<FolderDiffService>();
            services.AddTransient<FolderSynchronizationService>();
            services.AddTransient<FolderSyncService>();
            services.AddTransient<AppSettingsService>();
            services.AddTransient<AppSettingsForm>();
            services.AddTransient<FolderSyncForm>();

            // 設定可能在執行期被修改，提供工廠以便用最新設定重新建立相依物件
            services.AddSingleton<Func<FolderSyncService>>(sp => sp.GetRequiredService<FolderSyncService>);
            services.AddSingleton<Func<AppSettings>>(sp => sp.GetRequiredService<AppSettings>);
            services.AddSingleton<Func<AppSettingsForm>>(sp => sp.GetRequiredService<AppSettingsForm>);

            return services.BuildServiceProvider();
        }
    }
}
