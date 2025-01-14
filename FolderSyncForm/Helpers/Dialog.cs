
namespace FolderSyncForm.Helpers
{
    public class Dialog
    {
        public static DialogResult Alert(string text)
        {
            return DialogForm(text, MessageBoxButtons.OK);
        }

        public static DialogResult Error(Exception ex)
        {
            return DialogForm(ex.Message, MessageBoxButtons.OK);
        }

        public static bool Confirm(string text)
        {
            var result = DialogForm(text, MessageBoxButtons.OKCancel);
            return DialogResult.OK == result;
        }

        private static DialogResult DialogForm(string text, MessageBoxButtons buttons)
        {
            using (var dialog = new DialogForm(text, buttons))
            {
                return dialog.ShowDialog();
            }
        }

    }
}
