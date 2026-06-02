using FolderSyncForm.Composition;

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

            using var provider = new ServiceCollection().AddFolderSync().BuildServiceProvider();
            Application.Run(provider.GetRequiredService<Forms.FolderSyncForm>());
        }
    }
}
