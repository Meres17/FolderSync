namespace FolderSyncForm.Helpers
{
    /// <summary>
    /// 外觀的單一來源：統一字型、依角色配色的扁平按鈕，以及一致的輸入框與表格樣式。
    /// 僅調整子控件外觀，不更動 Form.Font/Padding 以免觸發 AutoScale 破版。
    /// </summary>
    public static class FormTheme
    {
        private const string FontFamily = "Microsoft JhengHei UI";
        private static readonly Font BaseFont = new(FontFamily, 11F);
        private static readonly Font HeaderFont = new(FontFamily, 11F, FontStyle.Bold);

        // 介面底色與文字
        private static readonly Color Surface = Color.White;
        private static readonly Color Text = Color.FromArgb(33, 37, 41);
        private static readonly Color OnAccent = Color.White;
        private static readonly Color InputBorder = Color.FromArgb(206, 212, 218);

        // 按鈕角色色（filled）
        private static readonly Color Primary = Color.FromArgb(37, 99, 235);   // 選擇/儲存
        private static readonly Color Success = Color.FromArgb(22, 163, 74);   // 檢視/列表
        private static readonly Color Danger = Color.FromArgb(220, 38, 38);    // 更新/還原
        private static readonly Color Neutral = Color.FromArgb(108, 117, 125); // 設定/刪除/取消

        // 表格色
        private static readonly Color HeaderBack = Color.FromArgb(245, 246, 248);
        private static readonly Color HeaderFore = Color.FromArgb(73, 80, 87);
        private static readonly Color GridLine = Color.FromArgb(233, 236, 239);
        private static readonly Color AlternateRow = Color.FromArgb(248, 249, 250);
        private static readonly Color SelectionBack = Color.FromArgb(219, 234, 254);
        private static readonly Color SelectionFore = Color.FromArgb(30, 64, 175);

        public static void Apply(Form form)
        {
            form.BackColor = Surface;
            Style(form);
        }

        private static void Style(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                control.Font = BaseFont;
                switch (control)
                {
                    case Button button:
                        StyleButton(button);
                        break;
                    case TextBox textBox:
                        textBox.BorderStyle = BorderStyle.FixedSingle;
                        break;
                    case ComboBox comboBox:
                        StyleComboBox(comboBox);
                        break;
                    case Label label:
                        label.ForeColor = Text;
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
            var color = RoleColor(button.Name);
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.BorderColor = color;
            button.FlatAppearance.MouseOverBackColor = Darken(color, 0.10);
            button.FlatAppearance.MouseDownBackColor = Darken(color, 0.20);
            button.BackColor = color;
            button.ForeColor = OnAccent;
            button.Cursor = Cursors.Hand;
            button.UseVisualStyleBackColor = false;
        }

        private static Color RoleColor(string name)
        {
            return name switch
            {
                "btnOverwrite" or "btnRestore" => Danger,
                "btnCompare" or "btnBackupList" => Success,
                "btnDeleteBackup" or "btnCancel" or "btnOpenSettings" => Neutral,
                _ => Primary, // btnSource / btnDest / btnSave / btnOK
            };
        }

        private static void StyleComboBox(ComboBox comboBox)
        {
            comboBox.FlatStyle = FlatStyle.Flat;
            comboBox.BackColor = Surface;
            comboBox.ForeColor = Text;
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
            grid.ColumnHeadersDefaultCellStyle.Font = HeaderFont;
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 8, 0);
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ColumnHeadersHeight = 38;

            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.DefaultCellStyle.Font = BaseFont;
            grid.DefaultCellStyle.Padding = new Padding(8, 0, 8, 0);
            grid.DefaultCellStyle.SelectionBackColor = SelectionBack;
            grid.DefaultCellStyle.SelectionForeColor = SelectionFore;
            grid.AlternatingRowsDefaultCellStyle.BackColor = AlternateRow;

            grid.RowTemplate.Height = 30;
            grid.RowHeadersVisible = false;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToResizeRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private static Color Darken(Color color, double amount)
        {
            return Color.FromArgb(
                color.A,
                (int)(color.R * (1 - amount)),
                (int)(color.G * (1 - amount)),
                (int)(color.B * (1 - amount)));
        }
    }
}
