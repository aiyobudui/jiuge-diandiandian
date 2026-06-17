using System;
using System.Drawing;
using System.Windows.Forms;

namespace JiuGeKeyClick
{
    public class KeyRecordDialog : Form
    {
        public string SelectedKey { get; private set; }
        private TextBox _txtKey;

        public KeyRecordDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "设置启动快捷键";
            this.Size = new Size(300, 180);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.KeyPreview = true;
            this.KeyDown += KeyRecordDialog_KeyDown;

            // 提示文字
            Label lblPrompt = new Label();
            lblPrompt.Text = "请按下新的启动快捷键 (F1-F12)：";
            lblPrompt.AutoSize = true;
            lblPrompt.Location = new Point(20, 22);
            lblPrompt.Font = new Font("Microsoft YaHei UI", 9);
            this.Controls.Add(lblPrompt);

            // 输入框
            TextBox txtKey = new TextBox();
            txtKey.ReadOnly = true;
            txtKey.Text = "等待按键...";
            txtKey.Font = new Font("Microsoft YaHei UI", 9);
            txtKey.Location = new Point(20, 55);
            txtKey.Size = new Size(255, 26);
            _txtKey = txtKey;
            this.Controls.Add(txtKey);

            // 按钮
            Button btnOK = new Button();
            btnOK.Text = "确定";
            btnOK.Size = new Size(75, 28);
            btnOK.Location = new Point(105, 100);
            btnOK.Font = new Font("Microsoft YaHei UI", 9);
            btnOK.Click += (s, e) =>
            {
                if (!string.IsNullOrEmpty(SelectedKey))
                    this.DialogResult = DialogResult.OK;
            };
            this.Controls.Add(btnOK);

            Button btnCancel = new Button();
            btnCancel.Text = "取消";
            btnCancel.Size = new Size(75, 28);
            btnCancel.Location = new Point(195, 100);
            btnCancel.Font = new Font("Microsoft YaHei UI", 9);
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            this.Controls.Add(btnCancel);

            // 默认焦点在输入框
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }

        private void KeyRecordDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode >= Keys.F1 && e.KeyCode <= Keys.F12)
            {
                SelectedKey = e.KeyCode.ToString();
                _txtKey.Text = SelectedKey;
            }
            e.Handled = true;
        }
    }
}
