using FolderSyncCore;

using FolderSyncForm.Extensions;

namespace FolderSyncForm
{
    public partial class MainForm : System.Windows.Forms.Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnSource_Click(object sender, EventArgs e)
        {
            txtSource.BindFolderDialog();
        }

        private void btnTarget_Click(object sender, EventArgs e)
        {
            txtTarget.BindFolderDialog();
        }

        private void btnCompare_Click(object sender, EventArgs e)
        {
            gvDiff.DataSource = FolderComparer.Diff(txtSource.Text, txtTarget.Text);
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            var dtos = (List<FileDTO>)gvDiff.DataSource;
            if (dtos == null) return;
            FolderComparer.Backup(dtos, txtSource.Text, txtTarget.Text);
            gvDiff.DataSource = FolderComparer.Diff(txtSource.Text, txtTarget.Text);
        }

        private void btnBackupList_Click(object sender, EventArgs e)
        {
            gvDiff.DataSource = FolderComparer.BackupFolders(txtSource.Text);
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            if (gvDiff.SelectedRows.Count == 0)
            {
                MessageBox.Show("�п�ܭn�٭쪺�ɮ�");
                return;
            }

            if (gvDiff.SelectedRows.Count > 1)
            {
                MessageBox.Show("�n�٭쪺�ɮץu���@��");
                return;
            }

            // ���o����� FolderDTO ����
            var dto = (FolderDTO)gvDiff.SelectedRows[0].DataBoundItem;

            FolderComparer.Restore(dto.������|, txtTarget.Text);
            MessageBox.Show("�٭즨�\");
        }
    }
}
