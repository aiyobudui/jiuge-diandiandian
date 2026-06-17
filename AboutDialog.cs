using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace JiuGeKeyClick
{
    public class AboutDialog : Form
    {
        private const string GITHUB_URL = "https://github.com/aiyobudui/jiuge-diandiandian";
        private const string QUARK_URL = "https://pan.quark.cn/s/280a143de78b";

        public AboutDialog()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
        }

        private void InitializeComponent()
        {
            this.Text = "关于";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.BackColor = Color.FromArgb(250, 250, 250);
            this.Padding = new Padding(0);
            this.ClientSize = new Size(440, 360);
            this.ControlBox = true;

            Font baseFont = new Font("微软雅黑", 9F);
            Font titleFont = new Font("微软雅黑", 12F, FontStyle.Bold);
            Font sectionFont = new Font("微软雅黑", 9F, FontStyle.Bold);
            Font linkFont = new Font("微软雅黑", 9F);

            int leftMargin = 28;
            int labelWidth = 92;
            int contentX = leftMargin + labelWidth;
            int y = 50;

            // ===== 名称 =====
            Label lblTitle = new Label
            {
                Text = "九歌键鼠助手",
                Location = new Point(leftMargin, y),
                AutoSize = true,
                Font = titleFont,
                ForeColor = Color.FromArgb(40, 40, 40)
            };
            this.Controls.Add(lblTitle);

            // 分隔线
            y += 40;
            Panel sep1 = new Panel
            {
                Location = new Point(leftMargin - 4, y),
                Size = new Size(this.ClientSize.Width - leftMargin * 2 + 8, 1),
                BackColor = Color.FromArgb(210, 210, 210)
            };
            this.Controls.Add(sep1);

            y += 16;

            // ===== 作者 =====
            Label lblAuthorLabel = new Label
            {
                Text = "作者：",
                Location = new Point(leftMargin, y),
                Size = new Size(labelWidth, 24),
                Font = sectionFont,
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(lblAuthorLabel);

            Label lblAuthor = new Label
            {
                Text = "对酒当歌",
                Location = new Point(contentX, y),
                Size = new Size(260, 24),
                Font = baseFont,
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(lblAuthor);

            y += 36;

            // 分隔线2
            Panel sep2 = new Panel
            {
                Location = new Point(leftMargin - 4, y),
                Size = new Size(this.ClientSize.Width - leftMargin * 2 + 8, 1),
                BackColor = Color.FromArgb(210, 210, 210)
            };
            this.Controls.Add(sep2);

            y += 16;

            // ===== GitHub 仓库 =====
            Label lblGitHubLabel = new Label
            {
                Text = "GitHub 仓库：",
                Location = new Point(leftMargin, y),
                Size = new Size(labelWidth, 24),
                Font = sectionFont,
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(lblGitHubLabel);

            LinkLabel lnkGitHub = new LinkLabel
            {
                Text = GITHUB_URL,
                Location = new Point(contentX, y),
                Size = new Size(300, 24),
                Font = linkFont,
                LinkColor = Color.FromArgb(30, 100, 200),
                ActiveLinkColor = Color.Red,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft
            };
            lnkGitHub.LinkBehavior = LinkBehavior.HoverUnderline;
            lnkGitHub.Links.Add(0, GITHUB_URL.Length, GITHUB_URL);
            lnkGitHub.LinkClicked += (s, e) =>
            {
                    try { Process.Start(new ProcessStartInfo(e.Link.LinkData.ToString()) { UseShellExecute = true }); }
                    catch { }
                };
            this.Controls.Add(lnkGitHub);

            y += 44;

            // ===== 夸克下载 =====
            Label lblDownloadLabel = new Label
            {
                Text = "夸克下载：",
                Location = new Point(leftMargin, y),
                Size = new Size(labelWidth, 24),
                Font = sectionFont,
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(lblDownloadLabel);

            LinkLabel lnkQuark = new LinkLabel
            {
                Text = QUARK_URL,
                Location = new Point(contentX, y),
                Size = new Size(300, 24),
                Font = linkFont,
                LinkColor = Color.FromArgb(30, 100, 200),
                ActiveLinkColor = Color.Red,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft
            };
            lnkQuark.LinkBehavior = LinkBehavior.HoverUnderline;
            lnkQuark.Links.Add(0, QUARK_URL.Length, QUARK_URL);
            lnkQuark.LinkClicked += (s, e) =>
            {
                try { Process.Start(new ProcessStartInfo(e.Link.LinkData.ToString()) { UseShellExecute = true }); }
                catch { }
            };
            this.Controls.Add(lnkQuark);

            y += 44;

            // ===== 海里免费资源分享 =====
            Label lblSiteLabel = new Label
            {
                Text = "资源分享：",
                Location = new Point(leftMargin, y),
                Size = new Size(labelWidth, 24),
                Font = sectionFont,
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(lblSiteLabel);

            LinkLabel lnkSite = new LinkLabel
            {
                Text = "www.haozy.top",
                Location = new Point(contentX, y),
                Size = new Size(300, 24),
                Font = linkFont,
                LinkColor = Color.FromArgb(30, 100, 200),
                ActiveLinkColor = Color.Red,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft
            };
            lnkSite.LinkBehavior = LinkBehavior.HoverUnderline;
            lnkSite.Links.Add(0, "www.haozy.top".Length, "https://www.haozy.top");
            lnkSite.LinkClicked += (s, e) =>
            {
                try { Process.Start(new ProcessStartInfo(e.Link.LinkData.ToString()) { UseShellExecute = true }); }
                catch { }
            };
            this.Controls.Add(lnkSite);

            y += 24;

            Label lblBackupLabel = new Label
            {
                Text = "防失联：",
                Location = new Point(leftMargin, y),
                Size = new Size(labelWidth, 24),
                Font = sectionFont,
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(lblBackupLabel);

            LinkLabel lnkBackup = new LinkLabel
            {
                Text = "link3.cc/hack",
                Location = new Point(contentX, y),
                Size = new Size(300, 24),
                Font = linkFont,
                LinkColor = Color.FromArgb(30, 100, 200),
                ActiveLinkColor = Color.Red,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft
            };
            lnkBackup.LinkBehavior = LinkBehavior.HoverUnderline;
            lnkBackup.Links.Add(0, "link3.cc/hack".Length, "https://link3.cc/hack");
            lnkBackup.LinkClicked += (s, e) =>
            {
                try { Process.Start(new ProcessStartInfo(e.Link.LinkData.ToString()) { UseShellExecute = true }); }
                catch { }
            };
            this.Controls.Add(lnkBackup);

            y += 44;

            // ===== 许可证 =====
            Label lblLicenseLabel = new Label
            {
                Text = "许可证：",
                Location = new Point(leftMargin, y),
                Size = new Size(labelWidth, 24),
                Font = sectionFont,
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(lblLicenseLabel);

            Label lblLicense = new Label
            {
                Text = "MIT License",
                Location = new Point(contentX, y),
                Size = new Size(200, 24),
                Font = baseFont,
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(lblLicense);

            y += 50;

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
