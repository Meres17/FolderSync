
namespace FolderSyncForm.Helpers
{
    public class MessageHelper
    {
        public static DialogResult Ok(string text)
        {
            return MessageBox.Show(text, "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static DialogResult Error(Exception ex)
        {
            return Error(ex.Message);
        }

        public static DialogResult Error(string text)
        {
            return MessageBox.Show(text, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static DialogResult Warning(string text)
        {
            return MessageBox.Show(text, "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }


    }
}
