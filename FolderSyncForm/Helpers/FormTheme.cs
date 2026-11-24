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
        private static readonly Color Canvas = Color.FromArgb(242, 244, 247); // 表單畫布（柔和淺灰）
        private static readonly Color Surface = Color.White;                  // 內容面（表格/輸入框）
        private static readonly Color Text = Color.FromArgb(33, 37, 41);
        private static readonly Color OnAccent = Color.White;
        private static readonly Color InputBorder = Color.FromArgb(206, 212, 218);

        // 按鈕角色色（同一套柔和冷色系，統一明度與彩度以求協調）
        private static readonly Color Primary = Color.FromArgb(61, 126, 187);  // 選擇/儲存（藍）
        private static readonly Color Success = Color.FromArgb(63, 167, 150);  // 檢視/列表（青）
        private static readonly Color Danger = Color.FromArgb(214, 99, 107);   // 更新/還原/刪除備份（珊瑚）
        private static readonly Color Neutral = Color.FromArgb(116, 129, 150); // 設定/取消（石板灰）

        // 表格色（中性灰，與按鈕冷色系一致）
        private static readonly Color HeaderBack = Color.FromArgb(244, 246, 249);
        private static readonly Color HeaderFore = Color.FromArgb(71, 85, 105);
        private static readonly Color GridLine = Color.FromArgb(231, 235, 240);
        private static readonly Color AlternateRow = Color.FromArgb(247, 249, 251);
        private static readonly Color SelectionBack = Color.FromArgb(225, 235, 245);
        private static readonly Color SelectionFore = Color.FromArgb(42, 88, 133);

        public static void Apply(Form form)
        {
            form.BackColor = Canvas;
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
                "btnOverwrite" or "btnRestore" or "btnDeleteBackup" or "btnConfirmOverwrite" => Danger,
                "btnCompare" or "btnBackupList" => Success,
                "btnCancel" or "btnOpenSettings" => Neutral,
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

            // 欄位自動填滿並隨視窗縮放；路徑類欄位分到較多寬度
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.DataBindingComplete += (_, _) =>
            {
                foreach (DataGridViewColumn column in grid.Columns)
                {
                    column.FillWeight = column.Name.Contains("路徑") ? 200 : 100;
                }
            };
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
