namespace FolderSyncForm.Helpers
{
    /// <summary>
    /// 集中套用現代扁平化外觀：扁平按鈕與表格樣式，沿用各控件既有配色與版面。
    /// </summary>
    public static class FormTheme
    {
        private static readonly Color Surface = Color.White;
        private static readonly Color HeaderBack = Color.FromArgb(245, 246, 248);
        private static readonly Color HeaderFore = Color.FromArgb(51, 51, 51);
        private static readonly Color GridLine = Color.FromArgb(235, 236, 238);
        private static readonly Color AlternateRow = Color.FromArgb(249, 250, 251);

        public static void Apply(Form form)
        {
            form.BackColor = Surface;
            Style(form);
        }

        private static void Style(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                switch (control)
                {
                    case Button button:
                        StyleButton(button);
                        break;
                    case DataGridView grid:
                        StyleGrid(grid);
                        break;
                }
                Style(control);
            }
        }

        private static void StyleButton(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Cursor = Cursors.Hand;
            button.UseVisualStyleBackColor = false;
        }

        private static void StyleGrid(DataGridView grid)
        {
            grid.BorderStyle = BorderStyle.None;
            grid.BackgroundColor = Surface;
            grid.GridColor = GridLine;
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.ColumnHeadersDefaultCellStyle.BackColor = HeaderBack;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = HeaderFore;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ColumnHeadersHeight = 36;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.AlternatingRowsDefaultCellStyle.BackColor = AlternateRow;
            grid.RowHeadersVisible = false;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToResizeRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }
    }
}
