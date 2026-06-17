using System;
using System.Drawing;
using System.Windows.Forms;

namespace JiuGeKeyClick
{
    public class HelpDialog : Form
    {
        public HelpDialog()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
        }

        private void InitializeComponent()
        {
            this.Text = "使用帮助";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.BackColor = Color.FromArgb(250, 250, 250);
            this.Padding = new Padding(0);
            this.ClientSize = new Size(480, 520);
            this.ControlBox = true;

            Font baseFont = new Font("微软雅黑", 9F);
            Font titleFont = new Font("微软雅黑", 12F, FontStyle.Bold);
            Font sectionFont = new Font("微软雅黑", 9F, FontStyle.Bold);
            Font linkFont = new Font("微软雅黑", 9F);

            int leftMargin = 28;
            int labelWidth = 80;
            int contentX = leftMargin + labelWidth + 10;
            int y = 30;

            // ===== 软件简介 =====
            Label lblTitle = new Label
            {
                Text = "软件简介",
                Location = new Point(leftMargin, y),
                AutoSize = true,
                Font = titleFont,
                ForeColor = Color.FromArgb(40, 40, 40)
            };
            this.Controls.Add(lblTitle);

            y += 32;
            Label lblIntro = new Label
            {
                Text = "九歌键鼠助手是一款轻量级键鼠宏工具，支持录制、编辑和循环执行键鼠动作序列。适用于游戏挂机、自动化办公等场景。",
                Location = new Point(leftMargin, y),
                Size = new Size(this.ClientSize.Width - leftMargin * 2, 40),
                Font = baseFont,
                ForeColor = Color.FromArgb(60, 60, 60)
            };
            this.Controls.Add(lblIntro);

            y += 55;

            // 分隔线
            Panel sep1 = new Panel
            {
                Location = new Point(leftMargin - 4, y),
                Size = new Size(this.ClientSize.Width - leftMargin * 2 + 8, 1),
                BackColor = Color.FromArgb(210, 210, 210)
            };
            this.Controls.Add(sep1);

            y += 16;

            // ===== 使用步骤 =====
            Label lblStepsTitle = new Label
            {
                Text = "使用方法",
                Location = new Point(leftMargin, y),
                AutoSize = true,
                Font = titleFont,
                ForeColor = Color.FromArgb(40, 40, 40)
            };
            this.Controls.Add(lblStepsTitle);

            y += 32;

            string[] steps = new string[]
            {
                "1. 点击「添加动作」按钮，创建新的键鼠动作",
                "2. 选择动作类型（键盘按键、鼠标点击等）",
                "3. 设置重复次数、前后间隔时间",
                "4. 重复以上步骤，添加更多动作",
                "5. 点击「保存配置」保存当前动作列表",
                "6. 按下快捷键（F8）启动/停止执行",
                "7. 执行过程中，动作会按顺序循环执行"
            };

            foreach (string step in steps)
            {
                Label lblStep = new Label
                {
                    Text = step,
                    Location = new Point(leftMargin, y),
                    Size = new Size(this.ClientSize.Width - leftMargin * 2, 20),
                    Font = baseFont,
                    ForeColor = Color.FromArgb(60, 60, 60)
                };
                this.Controls.Add(lblStep);
                y += 22;
            }

            y += 10;

            // 分隔线2
            Panel sep2 = new Panel
            {
                Location = new Point(leftMargin - 4, y),
                Size = new Size(this.ClientSize.Width - leftMargin * 2 + 8, 1),
                BackColor = Color.FromArgb(210, 210, 210)
            };
            this.Controls.Add(sep2);

            y += 16;

            // ===== 快捷键说明 =====
            Label lblHotkeyTitle = new Label
            {
                Text = "快捷键",
                Location = new Point(leftMargin, y),
                AutoSize = true,
                Font = titleFont,
                ForeColor = Color.FromArgb(40, 40, 40)
            };
            this.Controls.Add(lblHotkeyTitle);

            y += 32;

            Label lblHotkeyInfo = new Label
            {
                Text = "F8：启动/停止动作循环执行（可在设置中修改）",
                Location = new Point(leftMargin, y),
                Size = new Size(this.ClientSize.Width - leftMargin * 2, 20),
                Font = baseFont,
                ForeColor = Color.FromArgb(60, 60, 60)
            };
            this.Controls.Add(lblHotkeyInfo);

            y += 24;

            Label lblTipInfo = new Label
            {
                Text = "提示：程序最小化后会驻留到系统托盘，双击托盘图标可重新显示窗口。",
                Location = new Point(leftMargin, y),
                Size = new Size(this.ClientSize.Width - leftMargin * 2, 30),
                Font = new Font("微软雅黑", 8.5F),
                ForeColor = SystemColors.GrayText
            };
            this.Controls.Add(lblTipInfo);

            y += 40;

            // 底部按钮（右下角）
            int btnWidth = 90;
            int btnHeight = 32;
            Button btnOK = new Button
            {
                Text = "确定",
                Location = new Point(this.ClientSize.Width - btnWidth - 20, this.ClientSize.Height - btnHeight - 16),
                Size = new Size(btnWidth, btnHeight),
                Font = new Font("微软雅黑", 9.5F),
                UseVisualStyleBackColor = true,
                DialogResult = DialogResult.OK
            };
            btnOK.Click += (s, e) => this.Close();
            this.Controls.Add(btnOK);

            this.AcceptButton = btnOK;
            this.CancelButton = btnOK;
        }
    }
}
