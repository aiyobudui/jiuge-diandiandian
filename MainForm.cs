using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using System.Reflection;

namespace JiuGeKeyClick
{
    public partial class MainForm : Form
    {
        private string _startKey;
        private List<ActionItem> _actions = new List<ActionItem>();
        private ActionSimulator _simulator = new ActionSimulator();
        private NotifyIcon _notifyIcon = new NotifyIcon();
        private ContextMenuStrip _trayMenu = new ContextMenuStrip();
        private Timer _statusFlashTimer;
        private bool _flashState = false;
        private Icon _trayIconGray;
        private Icon _trayIconGreen;
        public MainForm()
        {
            InitializeComponent();
            _simulator.StatusChanged += Simulator_StatusChanged;
            _simulator.BeforeAction += Simulator_BeforeAction;
            _simulator.AfterAction += Simulator_AfterAction;
            LoadConfig();
            UpdateStatus();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Text = $"九歌键鼠助手 v{Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "1.2.0"}";
            this.Icon = LoadAppIcon();
            this.ShowIcon = false;
            this.Size = new Size(780, 580);
            this.MinimumSize = new Size(600, 420);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimizeBox = true;
            this.MaximizeBox = true;
            this.FormClosing += MainForm_FormClosing;
            this.Resize += MainForm_Resize;
            this.Shown += MainForm_Shown;

            // 加载托盘图标
            _trayIconGray = CreateTrayIcon(Color.Gray);
            _trayIconGreen = CreateTrayIcon(Color.LimeGreen);
            _notifyIcon.Icon = _trayIconGray;

            Font defaultFont = new Font("微软雅黑", 9);

            // ===== 主布局 =====
            TableLayoutPanel mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(10)
            };
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));   // 快捷键+状态
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));   // 动作列表
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));   // 工具栏
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));   // 动作数量
            this.Controls.Add(mainPanel);

            // ===== 第4行: 动作数量 =====
            Panel countPanel = new Panel { Dock = DockStyle.Fill };

            lblActionCount = new Label
            {
                Text = "动作数量：0",
                Location = new Point(4, 8),
                AutoSize = true,
                Font = defaultFont,
                ForeColor = SystemColors.GrayText
            };
            countPanel.Controls.Add(lblActionCount);

            // 关于按钮（右下角）
            Button btnAboutBottom = new Button
            {
                Text = "关于",
                Size = new Size(55, 23),
                Font = defaultFont,
                UseVisualStyleBackColor = true,
                Margin = new Padding(0),
            };
            btnAboutBottom.Click += (s, e) =>
            {
                using (AboutDialog dlg = new AboutDialog())
                    dlg.ShowDialog(this);
            };
            countPanel.Controls.Add(btnAboutBottom);

            // 右对齐关于按钮
            countPanel.Resize += (s, e) =>
            {
                btnAboutBottom.Location = new Point(countPanel.ClientSize.Width - btnAboutBottom.Width - 2, 4);
            };
            // 初始化位置
            btnAboutBottom.Location = new Point(countPanel.ClientSize.Width - btnAboutBottom.Width - 2, 4);

            mainPanel.Controls.Add(countPanel, 0, 3);

            // ===== 闪烁定时器 =====
            _statusFlashTimer = new Timer { Interval = 500 };
            _statusFlashTimer.Tick += (s, e) =>
            {
                _flashState = !_flashState;
                lblStatusDot.ForeColor = _flashState ? Color.LimeGreen : Color.FromArgb(180, 255, 180);
                _notifyIcon.Icon = _flashState ? _trayIconGreen : _trayIconGray;
            };

            // ===== 第1行: 快捷键 GroupBox =====
            GroupBox hotkeyGroup = new GroupBox
            {
                Text = "启动/停止快捷键",
                Dock = DockStyle.Fill,
                Font = defaultFont,
                Margin = new Padding(0, 8, 0, 0)
            };

            Label lblHotkeyPrefix = new Label
            {
                Text = "当前快捷键：",
                Location = new Point(14, 0),
                AutoSize = true,
                Font = defaultFont
            };
            hotkeyGroup.Controls.Add(lblHotkeyPrefix);

            lblHotkey = new Label
            {
                Text = "F8",
                Location = new Point(112, 0),
                Size = new Size(58, 30),
                Font = new Font("微软雅黑", 11, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(245, 245, 250)
            };
            hotkeyGroup.Controls.Add(lblHotkey);

            btnChangeHotkey = new Button
            {
                Text = "修改",
                Location = new Point(175, 0),
                Size = new Size(45, 26),
                Font = defaultFont
            };
            btnChangeHotkey.Click += btnHotkey_Click;
            hotkeyGroup.Controls.Add(btnChangeHotkey);

            Label lblHotkeyHint = new Label
            {
                Text = "按此键启动/停止循环",
                Location = new Point(228, 0),
                AutoSize = true,
                Font = defaultFont,
                ForeColor = SystemColors.GrayText
            };
            hotkeyGroup.Controls.Add(lblHotkeyHint);

            // 状态（右侧对齐）
            lblStatusDot = new Label
            {
                Text = "●",
                Size = new Size(20, 24),
                Font = new Font("微软雅黑", 12),
                ForeColor = Color.Red,
                TextAlign = ContentAlignment.MiddleCenter
            };
            hotkeyGroup.Controls.Add(lblStatusDot);

            lblStatus = new Label
            {
                Text = "当前状态：已停止",
                AutoSize = true,
                Font = new Font("微软雅黑", 10),
                ForeColor = Color.Red
            };
            hotkeyGroup.Controls.Add(lblStatus);

            // 垂直居中 + 状态右对齐：利用 DisplayRectangle 动态计算
            hotkeyGroup.Layout += (s, e) =>
            {
                int top = hotkeyGroup.DisplayRectangle.Top;
                int h = hotkeyGroup.DisplayRectangle.Height;
                // F8 标签和按钮垂直居中
                lblHotkey.Top = top + (h - lblHotkey.Height) / 2;
                btnChangeHotkey.Top = top + (h - btnChangeHotkey.Height) / 2;
                lblHotkeyPrefix.Top = lblHotkey.Top + 7;
                lblHotkeyHint.Top = lblHotkey.Top + 7;

                // 状态 → 右对齐（红点在最右），垂直居中
                int statusRight = hotkeyGroup.DisplayRectangle.Width - 4;
                int centerY = top + (h - lblStatusDot.Height) / 2;
                lblStatusDot.Location = new Point(statusRight - lblStatusDot.Width, centerY);
                lblStatus.Location = new Point(statusRight - lblStatusDot.Width - lblStatus.Width, centerY + (lblStatusDot.Height - lblStatus.Height) / 2);
            };

            mainPanel.Controls.Add(hotkeyGroup, 0, 0);

            // ===== 第2行: 动作列表 GroupBox =====
            GroupBox actionGroup = new GroupBox
            {
                Text = "动作列表",
                Dock = DockStyle.Fill,
                Font = defaultFont,
                Padding = new Padding(6, 18, 6, 6)
            };

            dgvActions = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = SystemColors.Window,
                BorderStyle = BorderStyle.Fixed3D,
                GridColor = SystemColors.ControlLight,
                Font = defaultFont
            };

            dgvActions.Columns.Add("Index", "序号");
            dgvActions.Columns.Add("Type", "动作类型");
            dgvActions.Columns.Add("Key", "按键/操作");
            dgvActions.Columns.Add("RepeatCount", "重复次数");
            dgvActions.Columns.Add("PreDelay", "前间隔");
            dgvActions.Columns.Add("Delay", "后间隔");
            dgvActions.Columns.Add("MousePos", "鼠标坐标");
            dgvActions.Columns.Add("Comment", "备注");
            dgvActions.Columns["Index"].FillWeight = 6;
            dgvActions.Columns["Type"].FillWeight = 12;
            dgvActions.Columns["Key"].FillWeight = 14;
            dgvActions.Columns["RepeatCount"].FillWeight = 10;
            dgvActions.Columns["PreDelay"].FillWeight = 8;
            dgvActions.Columns["Delay"].FillWeight = 8;
            dgvActions.Columns["MousePos"].FillWeight = 14;
            dgvActions.Columns["Comment"].FillWeight = 28;
            // 所有列内容居中
            foreach (DataGridViewColumn col in dgvActions.Columns)
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvActions.Columns["Index"].ReadOnly = true;
            dgvActions.Columns["Type"].ReadOnly = true;
            dgvActions.Columns["Key"].ReadOnly = true;
            dgvActions.Columns["RepeatCount"].ReadOnly = false;
            dgvActions.Columns["PreDelay"].ReadOnly = false;
            dgvActions.Columns["Delay"].ReadOnly = false;
            dgvActions.Columns["MousePos"].ReadOnly = true;
            dgvActions.Columns["Comment"].ReadOnly = false;
            dgvActions.CellEndEdit += dgvActions_CellEndEdit;
            dgvActions.CellDoubleClick += dgvActions_CellDoubleClick;
            dgvActions.DoubleClick += (s, e) =>
            {
                // 双击空白区域（表头下方但无行的区域）→ 添加
                Point client = dgvActions.PointToClient(MousePosition);
                var hit = dgvActions.HitTest(client.X, client.Y);
                if (hit.RowIndex < 0)
                    btnAdd_Click(s, e);
            };

            dgvActions.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 250);

            actionGroup.Controls.Add(dgvActions);
            mainPanel.Controls.Add(actionGroup, 0, 1);

            // ===== 第3行: 工具栏 =====
            ToolStrip toolbar = new ToolStrip
            {
                Dock = DockStyle.Fill,
                GripStyle = ToolStripGripStyle.Hidden,
                Font = defaultFont,
                RenderMode = ToolStripRenderMode.System,
                Padding = new Padding(4, 0, 0, 0)
            };

            btnAdd = new ToolStripButton("添加动作");
            btnAdd.Click += btnAdd_Click;

            btnEdit = new ToolStripButton("编辑");
            btnEdit.Click += btnEdit_Click;

            btnDelete = new ToolStripButton("删除");
            btnDelete.Click += btnDelete_Click;

            btnUp = new ToolStripButton("▲ 上移");
            btnUp.Click += btnUp_Click;

            btnDown = new ToolStripButton("▼ 下移");
            btnDown.Click += btnDown_Click;

            btnClear = new ToolStripButton("清空");
            btnClear.Click += btnClear_Click;

            toolbar.Items.Add(btnAdd);
            toolbar.Items.Add(btnEdit);
            toolbar.Items.Add(btnDelete);
            toolbar.Items.Add(btnUp);
            toolbar.Items.Add(btnDown);
            toolbar.Items.Add(btnClear);

            // 保存/加载按钮 → 右对齐
            ToolStripButton btnSaveCfg = new ToolStripButton("💾 保存配置");
            btnSaveCfg.Alignment = ToolStripItemAlignment.Right;
            btnSaveCfg.Click += btnSave_Click;
            toolbar.Items.Add(btnSaveCfg);

            ToolStripButton btnLoadCfg = new ToolStripButton("📂 加载配置");
            btnLoadCfg.Alignment = ToolStripItemAlignment.Right;
            btnLoadCfg.Click += btnImport_Click;
            toolbar.Items.Add(btnLoadCfg);

            mainPanel.Controls.Add(toolbar, 0, 2);

            // ===== 托盘图标 =====
            _notifyIcon.Visible = true;
            _notifyIcon.DoubleClick += NotifyIcon_DoubleClick;

            _trayMenu.Items.Add("📋 显示窗口", null, TrayMenu_ShowWindow);
            _trayMenu.Items.Add("-");
            _trayMenu.Items.Add("▶ 启动", null, TrayMenu_Start);
            _trayMenu.Items.Add("-");
            _trayMenu.Items.Add("⏹ 停止", null, TrayMenu_Stop);
            _trayMenu.Items.Add("-");
            _trayMenu.Items.Add("ℹ 关于", null, TrayMenu_About);
            _trayMenu.Items.Add("-");
            _trayMenu.Items.Add("❌ 退出", null, TrayMenu_Exit);
            _notifyIcon.ContextMenuStrip = _trayMenu;

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        // 加载图标：直接从 EXE 自身提取（<ApplicationIcon> 已嵌入编译）
        private Icon LoadAppIcon()
        {
            try
            {
                string exePath = Application.ExecutablePath;
                if (!string.IsNullOrEmpty(exePath) && File.Exists(exePath))
                    return Icon.ExtractAssociatedIcon(exePath);
            }
            catch { }

            // fallback
            using (Bitmap bmp = new Bitmap(32, 32))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(90, 170, 250)))
                    g.FillEllipse(brush, 2, 2, 28, 28);
                return Icon.FromHandle(bmp.GetHicon());
            }
        }

        // 生成纯色圆形托盘图标
        private Icon CreateTrayIcon(Color color)
        {
            int size = 32;
            using (Bitmap bmp = new Bitmap(size, size))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                using (SolidBrush brush = new SolidBrush(color))
                {
                    g.FillEllipse(brush, 2, 2, size - 4, size - 4);
                }
                return Icon.FromHandle(bmp.GetHicon());
            }
        }

        private Label lblStatusDot;
        private Label lblStatus;
        private Label lblHotkey;
        private Label lblActionCount;
        private DataGridView dgvActions;
        private ToolStripButton btnAdd;
        private ToolStripButton btnEdit;
        private ToolStripButton btnDelete;
        private ToolStripButton btnUp;
        private ToolStripButton btnDown;
        private ToolStripButton btnClear;
        private Button btnChangeHotkey;
        private bool _initialized = false;
        private bool _isExiting = false;

        private void LoadConfig()
        {
            ConfigService.LoadConfig(out _startKey, out _actions);
            RefreshActionGrid();
            RegisterHotkey();
            lblHotkey.Text = _startKey;
        }

        private void RefreshActionGrid()
        {
            dgvActions.Rows.Clear();
            for (int i = 0; i < _actions.Count; i++)
            {
                ActionItem action = _actions[i];
                string mousePos = action.Type == ActionType.MouseLeft || action.Type == ActionType.MouseRight || action.Type == ActionType.MouseMiddle
                    ? $"({action.MouseX}, {action.MouseY})" : "-";
                dgvActions.Rows.Add(i + 1, action.TypeDisplayName, action.KeyDisplayName, action.RepeatCount, action.PreDelay, action.Delay, mousePos, action.Comment);
            }
            UpdateStatusBar();
        }

        private void RegisterHotkey()
        {
            NativeMethods.UnregisterHotKey(this.Handle, NativeMethods.HOTKEY_ID);

            Keys key = ParseKeyName(_startKey);
            if (key != Keys.None)
            {
                bool success = NativeMethods.RegisterHotKey(this.Handle, NativeMethods.HOTKEY_ID, NativeMethods.MOD_NONE, (uint)key);
                if (!success)
                {
                    MessageBox.Show("快捷键注册失败，可能已被其他程序占用", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private Keys ParseKeyName(string keyName)
        {
            if (Enum.TryParse(keyName, true, out Keys result))
            {
                return result;
            }
            return Keys.None;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == NativeMethods.WM_HOTKEY && m.WParam.ToInt32() == NativeMethods.HOTKEY_ID)
            {
                ToggleRunning();
            }
            base.WndProc(ref m);
        }

        private void ToggleRunning()
        {
            if (_simulator.IsRunning)
            {
                StopRunning();
            }
            else
            {
                StartRunning();
            }
        }

        private void StartRunning()
        {
            _simulator.SetActions(_actions);
            _simulator.Start();
        }

        private void StopRunning()
        {
            _simulator.Stop();
        }

        private void UpdateStatus()
        {
            if (_simulator.IsRunning)
            {
                lblStatus.Text = "当前状态：运行中";
                lblStatus.ForeColor = Color.LimeGreen;
                lblStatusDot.ForeColor = Color.LimeGreen;
                _statusFlashTimer.Start();
                _notifyIcon.Icon = _trayIconGreen;
                if (_initialized)
                    _notifyIcon.ShowBalloonTip(1000, "九歌键鼠助手", "程序已启动", ToolTipIcon.Info);
            }
            else
            {
                _statusFlashTimer.Stop();
                lblStatus.Text = "当前状态：已停止";
                lblStatus.ForeColor = Color.Red;
                lblStatusDot.ForeColor = Color.Red;
                lblStatusDot.Visible = true;
                _notifyIcon.Icon = _trayIconGray;
                if (_initialized)
                    _notifyIcon.ShowBalloonTip(1000, "九歌键鼠助手", "程序已停止", ToolTipIcon.Info);
            }
            UpdateStatusBar();
        }

        private void UpdateStatusBar()
        {
            lblActionCount.Text = $"动作数量：{_actions.Count}";
        }

        private void Simulator_StatusChanged(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(UpdateStatus));
            }
            else
            {
                UpdateStatus();
            }
        }

        private void Simulator_BeforeAction(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => NativeMethods.UnregisterHotKey(this.Handle, NativeMethods.HOTKEY_ID)));
            }
            else
            {
                NativeMethods.UnregisterHotKey(this.Handle, NativeMethods.HOTKEY_ID);
            }
        }

        private void Simulator_AfterAction(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(RegisterHotkey));
            }
            else
            {
                RegisterHotkey();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (ActionEditDialog dlg = new ActionEditDialog())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    _actions.Add(dlg.Action);
                    RefreshActionGrid();
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvActions.SelectedRows.Count > 0)
            {
                int index = dgvActions.SelectedRows[0].Index;
                if (index >= 0 && index < _actions.Count)
                {
                    using (ActionEditDialog dlg = new ActionEditDialog(_actions[index]))
                    {
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            _actions[index] = dlg.Action;
                            RefreshActionGrid();
                        }
                    }
                }
            }
            else if (_actions.Count == 0)
            {
                btnAdd_Click(sender, e);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvActions.SelectedRows.Count > 0)
            {
                int index = dgvActions.SelectedRows[0].Index;
                if (index >= 0 && index < _actions.Count)
                {
                    if (MessageBox.Show($"确定要删除第 {index + 1} 个动作吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        _actions.RemoveAt(index);
                        RefreshActionGrid();
                    }
                }
            }
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            if (dgvActions.SelectedRows.Count > 0)
            {
                int index = dgvActions.SelectedRows[0].Index;
                if (index > 0)
                {
                    ActionItem temp = _actions[index];
                    _actions[index] = _actions[index - 1];
                    _actions[index - 1] = temp;
                    RefreshActionGrid();
                    dgvActions.Rows[index - 1].Selected = true;
                }
            }
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            if (dgvActions.SelectedRows.Count > 0)
            {
                int index = dgvActions.SelectedRows[0].Index;
                if (index < _actions.Count - 1)
                {
                    ActionItem temp = _actions[index];
                    _actions[index] = _actions[index + 1];
                    _actions[index + 1] = temp;
                    RefreshActionGrid();
                    dgvActions.Rows[index + 1].Selected = true;
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定要清空所有动作吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _actions.Clear();
                RefreshActionGrid();
            }
        }

        private void btnHotkey_Click(object sender, EventArgs e)
        {
            using (KeyRecordDialog dlg = new KeyRecordDialog())
            {
                if (dlg.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(dlg.SelectedKey))
                {
                    _startKey = dlg.SelectedKey;
                    lblHotkey.Text = _startKey;
                    RegisterHotkey();
                    ConfigService.SaveConfig(_startKey, _actions);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "INI配置文件 (*.ini)|*.ini|所有文件 (*.*)|*.*";
                dlg.DefaultExt = "ini";
                dlg.FileName = "九歌键鼠助手配置.ini";
                dlg.Title = "保存配置";
                dlg.InitialDirectory = Application.ExecutablePath != null
                    ? Path.GetDirectoryName(Application.ExecutablePath)
                    : "";

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        ConfigService.SaveConfig(_startKey, _actions);
                        ConfigService.SaveToFile(dlg.FileName, _startKey, _actions);
                        MessageBox.Show("配置已保存", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("保存配置失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "INI配置文件 (*.ini)|*.ini|所有文件 (*.*)|*.*";
                dlg.Title = "选择要加载的配置文件";
                dlg.RestoreDirectory = true;
                dlg.InitialDirectory = Application.StartupPath ?? "";

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        // 先验证格式：尝试读取文件
                        if (!ConfigService.LoadFromFile(dlg.FileName, out _, out List<ActionItem> testActions))
                        {
                            MessageBox.Show("配置文件格式不正确，无法读取", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // 格式正确，执行导入
                        if (ConfigService.ImportConfig(dlg.FileName, out string errorMessage))
                        {
                            LoadConfig();
                            MessageBox.Show("配置加载成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("加载失败: " + errorMessage, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("加载配置时发生错误: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void dgvActions_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < _actions.Count)
            {
                if (e.ColumnIndex == dgvActions.Columns["RepeatCount"].Index)
                {
                    if (int.TryParse(dgvActions.Rows[e.RowIndex].Cells["RepeatCount"].Value?.ToString(), out int repeatCount))
                    {
                        _actions[e.RowIndex].RepeatCount = Math.Max(1, repeatCount);
                    }
                }
                else if (e.ColumnIndex == dgvActions.Columns["PreDelay"].Index)
                {
                    if (int.TryParse(dgvActions.Rows[e.RowIndex].Cells["PreDelay"].Value?.ToString(), out int preDelay))
                    {
                        _actions[e.RowIndex].PreDelay = Math.Max(0, preDelay);
                    }
                }
                else if (e.ColumnIndex == dgvActions.Columns["Delay"].Index)
                {
                    if (int.TryParse(dgvActions.Rows[e.RowIndex].Cells["Delay"].Value?.ToString(), out int delay))
                    {
                        _actions[e.RowIndex].Delay = Math.Max(0, delay);
                    }
                }
                else if (e.ColumnIndex == dgvActions.Columns["Comment"].Index)
                {
                    _actions[e.RowIndex].Comment = dgvActions.Rows[e.RowIndex].Cells["Comment"].Value?.ToString() ?? string.Empty;
                }
            }
        }

        private void dgvActions_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // 双击已有行 → 编辑
            if (e.RowIndex >= 0 && e.RowIndex < _actions.Count)
            {
                dgvActions.Rows[e.RowIndex].Selected = true;
                btnEdit_Click(sender, e);
            }
            // 双击空白行（RowIndex >= 数据行数）→ 添加
            else if (e.RowIndex >= 0)
            {
                btnAdd_Click(sender, e);
            }
            // 双击表头/空白区域（RowIndex = -1）→ 由 DoubleClick 事件处理
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_isExiting)
                return;

            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
                _notifyIcon.ShowBalloonTip(1000, "九歌键鼠助手", "程序已最小化到托盘", ToolTipIcon.Info);
            }
            else
            {
                StopRunning();
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            _initialized = true;
            _notifyIcon.ShowBalloonTip(1000, "九歌键鼠助手", "程序已启动", ToolTipIcon.Info);
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void TrayMenu_ShowWindow(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void TrayMenu_Start(object sender, EventArgs e)
        {
            StartRunning();
        }

        private void TrayMenu_Stop(object sender, EventArgs e)
        {
            StopRunning();
        }

        private void TrayMenu_About(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            using (AboutDialog dlg = new AboutDialog())
                dlg.ShowDialog(this);
        }

        private void TrayMenu_Exit(object sender, EventArgs e)
        {
            _isExiting = true;
            _initialized = false;
            _statusFlashTimer.Stop();
            StopRunning();
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
            Application.Exit();
        }
    }
}
