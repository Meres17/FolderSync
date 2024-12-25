namespace FolderSyncForm.Extensions
{
    public static class ControlExtensions
    {
        public static void BindFolderDialog(this TextBox txt)
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() is DialogResult.OK)
            {
                txt.Text = dialog.SelectedPath;
            }
        }
    }
}
