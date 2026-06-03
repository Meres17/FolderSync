using Microsoft.Extensions.DependencyInjection;

namespace FolderSyncForm.Composition
{
    /// <summary>
    /// 組裝根：依層註冊相依物件，讓 Program 維持精簡。
    /// </summary>
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFolderSync(this IServiceCollection services)
        {
            return services
                .AddInfrastructure()
                .AddDomain()
                .AddApplication()
                .AddPresentation();
        }

        private static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton(TimeProvider.System);
            services.AddSingleton<IAppSettingsRepository, JsonAppSettingsRepository>();
            services.AddSingleton<IAuditLog>(sp => new FileAuditLog(clock: sp.GetRequiredService<TimeProvider>()));
            services.AddSingleton<IDirectoryProbe, FileSystemDirectoryProbe>();
            services.AddSingleton<IFolderScanner, FolderScanner>();
            services.AddSingleton<IFileOperations, FileOperations>();
            services.AddSingleton<ISiteController, SiteController>();
            services.AddTransient<IBackupRepository, FileSystemBackupRepository>();
            services.AddTransient<IFolderSynchronizerFactory, FolderSynchronizerFactory>();
            return services;
        }

        private static IServiceCollection AddDomain(this IServiceCollection services)
        {
            services.AddSingleton<AppSettingsProvider>();
            services.AddSingleton<FolderComparer>();
            services.AddTransient<FolderDiffService>();
            return services;
        }

        private static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddTransient<FolderSynchronizationService>();
            services.AddTransient<FolderSyncService>();
            services.AddTransient<AppSettingsService>();
            return services;
        }

        private static IServiceCollection AddPresentation(this IServiceCollection services)
        {
            services.AddTransient<Forms.FolderSyncForm>();
            return services;
        }
    }
}
