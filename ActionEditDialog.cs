using System;
using System.Drawing;
using System.Windows.Forms;

namespace JiuGeKeyClick
{
    public class ActionEditDialog : Form
    {
        private ActionItem _action;
        private bool _isRecording = false;
        private ComboBox _cboType;
        private TextBox _txtKey;
        private Panel pnlKeyHighlight;
        private NumericUpDown _numRepeatCount;
        private NumericUpDown _numPreDelay;
        private NumericUpDown _numDelay;
        private TextBox _txtMouseX;
        private TextBox _txtMouseY;
        private Label _lblMouseX;
        private Label _lblMouseY;
        private Label _lblMouseTitle;
        private Label _lblCaptureHint;
        private TextBox _txtComment;
        private Button _btnOK;
        private Button _btnCancel;

        public ActionItem Action => _action;

        public ActionEditDialog() : this(new ActionItem())
        {
        }

        public ActionEditDialog(ActionItem action)
        {
            _action = action.Clone();
            InitializeComponent();
            this.Shown += ActionEditDialog_Shown;
            this.FormClosing += ActionEditDialog_FormClosing;
        }

        private void InitializeComponent()
        {
            this.Text = (_action.Type == ActionType.Keyboard && string.IsNullOrEmpty(_action.Key))
                ? "添加动作" : "编辑动作";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.KeyPreview = true;
            this.KeyDown += ActionEditDialog_KeyDown;
            this.Padding = new Padding(16);

            int leftMargin = 20;
            int labelWidth = 100;
            int inputWidth = 280;
            int rowHeight = 36;
            int currentY = 16;

            Font labelFont = new Font("微软雅黑", 9);
            Font inputFont = new Font("微软雅黑", 10);
            Font hintFont = new Font("微软雅黑", 8F, FontStyle.Italic);

            // ===== 顶部提示：添加/编辑标题 =====
            // (用窗口标题)

            // ===== 动作类型 =====
            Label lblType = new Label
            {
                Text = "动作类型：",
                Location = new Point(leftMargin, currentY + 8),
                Size = new Size(labelWidth, 24),
                Font = labelFont,
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(lblType);

            _cboType = new ComboBox
            {
                Location = new Point(leftMargin + labelWidth + 8, currentY + 4),
                Size = new Size(inputWidth, 28),
                Font = inputFont,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cboType.Items.Add("键盘按键");
            _cboType.Items.Add("鼠标左键");
            _cboType.Items.Add("鼠标右键");
            _cboType.Items.Add("鼠标中键");
            _cboType.SelectedIndex = (int)_action.Type;
            _cboType.SelectedIndexChanged += _cboType_SelectedIndexChanged;
            this.Controls.Add(_cboType);

            currentY += rowHeight + 8;

            // ===== 目标按键（重点高亮） =====
            Label lblKey = new Label
            {
                Text = "目标按键：",
                Location = new Point(leftMargin, currentY + 8),
                Size = new Size(labelWidth, 24),
                Font = new Font("微软雅黑", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 102, 204),
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(lblKey);

            // 目标按键外框 Panel（独特样式）
            pnlKeyHighlight = new Panel
            {
                Location = new Point(leftMargin + labelWidth + 4, currentY),
                Size = new Size(inputWidth + 10, 36),
                BackColor = Color.FromArgb(255, 248, 220),   // 浅黄色背景
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(2)
            };
            this.Controls.Add(pnlKeyHighlight);

            _txtKey = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = inputFont,
                ReadOnly = true,
                Text = _action.Type == ActionType.Keyboard
                    ? (string.IsNullOrEmpty(_action.Key) ? "点击此处，按下按键" : _action.Key)
                    : _action.KeyDisplayName,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(200, 50, 50)       // 红褐色文字突出
            };
            _txtKey.Click += _txtKey_Click;
            _txtKey.Leave += (s, ev) =>
            {
                _isRecording = false;
                pnlKeyHighlight.BackColor = Color.FromArgb(255, 248, 220);
                _txtKey.ForeColor = Color.FromArgb(200, 50, 50);
            };
            pnlKeyHighlight.Controls.Add(_txtKey);

            currentY += rowHeight + 8;

            // ===== 重复次数 =====
            Label lblRepeatCount = new Label
            {
                Text = "重复次数：",
                Location = new Point(leftMargin, currentY + 8),
                Size = new Size(labelWidth, 24),
                Font = labelFont,
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(lblRepeatCount);

            _numRepeatCount = new NumericUpDown
            {
                Location = new Point(leftMargin + labelWidth + 8, currentY + 4),
                Size = new Size(80, 28),
                Font = inputFont,
                Minimum = 1,
                Maximum = 99999,
                Value = _action.RepeatCount
            };
            this.Controls.Add(_numRepeatCount);

            Label lblRepeatUnit = new Label
            {
                Text = "次",
                Location = new Point(leftMargin + labelWidth + 96, currentY + 8),
                Size = new Size(30, 24),
                Font = labelFont,
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(lblRepeatUnit);

            currentY += rowHeight + 8;

            // ===== 前间隔 =====
            Label lblPreDelay = new Label
            {
                Text = "前间隔：",
                Location = new Point(leftMargin, currentY + 8),
                Size = new Size(labelWidth, 24),
                Font = labelFont,
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(lblPreDelay);

            _numPreDelay = new NumericUpDown
            {
                Location = new Point(leftMargin + labelWidth + 8, currentY + 4),
                Size = new Size(80, 28),
                Font = inputFont,
                Minimum = 0,
                Maximum = 60000,
                Value = _action.PreDelay
            };
            this.Controls.Add(_numPreDelay);

            Label lblPreDelayHint = new Label
            {
                Text = "毫秒（执行前等待）",
                Location = new Point(leftMargin + labelWidth + 96, currentY + 8),
                Size = new Size(180, 24),
                Font = new Font("微软雅黑", 8.5F),
                ForeColor = SystemColors.GrayText,
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(lblPreDelayHint);

            currentY += rowHeight + 8;

            // ===== 后间隔 =====
            Label lblDelay = new Label
            {
                Text = "后间隔：",
                Location = new Point(leftMargin, currentY + 8),
                Size = new Size(labelWidth, 24),
                Font = labelFont,
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(lblDelay);

            _numDelay = new NumericUpDown
            {
                Location = new Point(leftMargin + labelWidth + 8, currentY + 4),
                Size = new Size(80, 28),
                Font = inputFont,
                Minimum = 0,
                Maximum = 60000,
                Value = _action.Delay
            };
            this.Controls.Add(_numDelay);

            Label lblDelayHint = new Label
            {
                Text = "毫秒（执行后等待）",
                Location = new Point(leftMargin + labelWidth + 96, currentY + 8),
                Size = new Size(180, 24),
                Font = new Font("微软雅黑", 8.5F),
                ForeColor = SystemColors.GrayText,
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(lblDelayHint);

            currentY += rowHeight + 8;

            // ===== 鼠标坐标 =====
            Label lblMousePos = new Label
            {
                Text = "鼠标坐标：",
                Location = new Point(leftMargin, currentY + 8),
                Size = new Size(labelWidth, 24),
                Font = labelFont,
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(lblMousePos);

            // X
            _lblMouseX = new Label
            {
                Text = "X：",
                Location = new Point(leftMargin + labelWidth + 8, currentY + 8),
                Size = new Size(22, 24),
                Font = labelFont,
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(_lblMouseX);

            _txtMouseX = new TextBox
            {
                Location = new Point(leftMargin + labelWidth + 30, currentY + 4),
                Size = new Size(80, 28),
                Font = inputFont,
                Text = _action.MouseX.ToString()
            };
            this.Controls.Add(_txtMouseX);

            // Y
            _lblMouseY = new Label
            {
                Text = "Y：",
                Location = new Point(leftMargin + labelWidth + 120, currentY + 8),
                Size = new Size(22, 24),
                Font = labelFont,
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(_lblMouseY);

            _txtMouseY = new TextBox
            {
                Location = new Point(leftMargin + labelWidth + 142, currentY + 4),
                Size = new Size(80, 28),
                Font = inputFont,
                Text = _action.MouseY.ToString()
            };
            this.Controls.Add(_txtMouseY);

            // 捕获提示（始终显示，键盘类型时灰显）
            _lblCaptureHint = new Label
            {
                Location = new Point(leftMargin + labelWidth + 8, currentY + 36),
                Size = new Size(inputWidth, 18),
                Font = hintFont,
                ForeColor = Color.FromArgb(100, 100, 180),
                Text = "💡 提示：在屏幕任意位置按下 Ctrl + Shift + Z 可捕获当前鼠标坐标"
            };
            this.Controls.Add(_lblCaptureHint);

            _lblMouseTitle = lblMousePos;

            currentY += 60;

            // ===== 备注 =====
            Label lblComment = new Label
            {
                Text = "备注：",
                Location = new Point(leftMargin, currentY + 4),
                Size = new Size(labelWidth, 24),
                Font = labelFont,
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(lblComment);

            _txtComment = new TextBox
            {
                Location = new Point(leftMargin + labelWidth + 8, currentY),
                Size = new Size(inputWidth, 52),
                Font = inputFont,
                Multiline = true,
                Text = _action.Comment
            };
            this.Controls.Add(_txtComment);

            currentY += 66;

            // ===== 按钮 =====
            // 客户区宽度固定为 460，直接用它计算居中
            int btnWidth = 90;
            int btnCenterX = (460 - btnWidth * 2 - 20) / 2;  // = 130
            int btnY = currentY + 16;

            _btnOK = new Button
            {
                Text = "确定",
                Location = new Point(btnCenterX, btnY),
                Size = new Size(btnWidth, 34),
                Font = new Font("微软雅黑", 10),
                UseVisualStyleBackColor = true,
                DialogResult = DialogResult.OK
            };
            _btnOK.Click += (s, e) =>
            {
                _action.Type = (ActionType)_cboType.SelectedIndex;
                _action.RepeatCount = (int)_numRepeatCount.Value;
                _action.PreDelay = (int)_numPreDelay.Value;
                _action.Delay = (int)_numDelay.Value;
                _action.Comment = _txtComment.Text;
                if (int.TryParse(_txtMouseX.Text, out int mx)) _action.MouseX = mx;
                if (int.TryParse(_txtMouseY.Text, out int my)) _action.MouseY = my;
                this.DialogResult = DialogResult.OK;
            };
            this.Controls.Add(_btnOK);

            _btnCancel = new Button
            {
                Text = "取消",
                Location = new Point(btnCenterX + btnWidth + 20, btnY),
                Size = new Size(btnWidth, 34),
                Font = new Font("微软雅黑", 10),
                UseVisualStyleBackColor = true,
                DialogResult = DialogResult.Cancel
            };
            this.Controls.Add(_btnCancel);

            this.AcceptButton = _btnOK;
            this.CancelButton = _btnCancel;

            UpdateMouseControlsVisibility();

            // 固定窗口大小（客户区 460×475）
            this.ClientSize = new Size(460, 475);
        }

        private void UpdateMouseControlsVisibility()
        {
            bool isMouseAction = _cboType.SelectedIndex == (int)ActionType.MouseLeft ||
                                _cboType.SelectedIndex == (int)ActionType.MouseRight ||
                                _cboType.SelectedIndex == (int)ActionType.MouseMiddle;

            // 坐标区域始终显示，键盘类型时禁用（灰色）
            _lblMouseTitle.Enabled = isMouseAction;
            _lblMouseX.Enabled = isMouseAction;
            _lblMouseY.Enabled = isMouseAction;
            _txtMouseX.Enabled = isMouseAction;
            _txtMouseY.Enabled = isMouseAction;
            _lblCaptureHint.Enabled = isMouseAction;
        }

        private void _cboType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActionType type = (ActionType)_cboType.SelectedIndex;
            if (type == ActionType.Keyboard)
            {
                _txtKey.Text = string.IsNullOrEmpty(_action.Key) ? "点击此处，按下按键" : _action.Key;
                _txtKey.Enabled = true;
                pnlKeyHighlight.BackColor = Color.FromArgb(255, 248, 220);
            }
            else
            {
                switch (type)
                {
                    case ActionType.MouseLeft: _txtKey.Text = "左键单击"; break;
                    case ActionType.MouseRight: _txtKey.Text = "右键单击"; break;
                    case ActionType.MouseMiddle: _txtKey.Text = "中键单击"; break;
                }
                _txtKey.Enabled = false;
            }
            UpdateMouseControlsVisibility();
        }

        private void _txtKey_Click(object sender, EventArgs e)
        {
            if (_cboType.SelectedIndex == (int)ActionType.Keyboard)
            {
                _isRecording = true;
                _txtKey.Text = "请按下键盘按键...";
                pnlKeyHighlight.BackColor = Color.FromArgb(255, 230, 180);  // 橙黄色闪烁
                _txtKey.ForeColor = Color.FromArgb(200, 80, 0);
            }
        }

        private void ActionEditDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.Shift && e.KeyCode == Keys.Z)
            {
                CaptureMousePosition();
                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }

            if (_isRecording && _cboType.SelectedIndex == (int)ActionType.Keyboard)
            {
                string keyName = GetKeyName(e);
                if (!string.IsNullOrEmpty(keyName))
                {
                    _action.Key = keyName;
                    _txtKey.Text = keyName;
                    _isRecording = false;
                    // 恢复高亮样式
                    pnlKeyHighlight.BackColor = Color.FromArgb(255, 248, 220);
                    _txtKey.ForeColor = Color.FromArgb(200, 50, 50);
                }
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private string GetKeyName(KeyEventArgs e)
        {
            string modifiers = "";
            if (e.Control) modifiers += "Ctrl+";
            if (e.Alt) modifiers += "Alt+";
            if (e.Shift) modifiers += "Shift+";

            string keyName = "";
            Keys key = e.KeyCode;

            switch (key)
            {
                case Keys.Space: keyName = "Space"; break;
                case Keys.Enter: keyName = "Enter"; break;
                case Keys.Tab: keyName = "Tab"; break;
                case Keys.Back: keyName = "Backspace"; break;
                case Keys.Delete: keyName = "Delete"; break;
                case Keys.Insert: keyName = "Insert"; break;
                case Keys.Home: keyName = "Home"; break;
                case Keys.End: keyName = "End"; break;
                case Keys.PageUp: keyName = "PageUp"; break;
                case Keys.PageDown: keyName = "PageDown"; break;
                case Keys.Up: keyName = "Up"; break;
                case Keys.Down: keyName = "Down"; break;
                case Keys.Left: keyName = "Left"; break;
                case Keys.Right: keyName = "Right"; break;
                case Keys.Escape: keyName = "Escape"; break;
                case Keys.Capital: keyName = "CapsLock"; break;
                case Keys.NumLock: keyName = "NumLock"; break;
                case Keys.Scroll: keyName = "ScrollLock"; break;
                case Keys.PrintScreen: keyName = "PrintScreen"; break;
                case Keys.Pause: keyName = "Pause"; break;
                case Keys.LWin:
                case Keys.RWin: keyName = "Win"; break;
                case Keys.ShiftKey:
                case Keys.LShiftKey:
                case Keys.RShiftKey: keyName = "Shift"; break;
                case Keys.ControlKey:
                case Keys.LControlKey:
                case Keys.RControlKey: keyName = "Ctrl"; break;
                case Keys.Menu:
                case Keys.LMenu:
                case Keys.RMenu: keyName = "Alt"; break;
                default:
                    if (key >= Keys.F1 && key <= Keys.F24)
                        keyName = key.ToString();
                    else if (key >= Keys.A && key <= Keys.Z)
                        keyName = key.ToString();
                    else if (key >= Keys.D0 && key <= Keys.D9)
                        keyName = key.ToString().Replace("D", "");
                    else if (key >= Keys.NumPad0 && key <= Keys.NumPad9)
                        keyName = "NumPad" + (key - Keys.NumPad0);
                    else if (key >= Keys.Multiply && key <= Keys.Divide)
                    {
                        switch (key)
                        {
                            case Keys.Multiply: keyName = "NumPad*"; break;
                            case Keys.Add: keyName = "NumPad+"; break;
                            case Keys.Subtract: keyName = "NumPad-"; break;
                            case Keys.Decimal: keyName = "NumPad."; break;
                            case Keys.Divide: keyName = "NumPad/"; break;
                        }
                    }
                    else
                    {
                        keyName = key.ToString();
                    }
                    break;
            }

            if (string.IsNullOrEmpty(keyName))
                return null;

            return modifiers + keyName;
        }

        private void CaptureMousePosition()
        {
            NativeMethods.POINT point;
            if (NativeMethods.GetCursorPos(out point))
            {
                _txtMouseX.Text = point.X.ToString();
                _txtMouseY.Text = point.Y.ToString();
                _action.MouseX = point.X;
                _action.MouseY = point.Y;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.Shift | Keys.Z))
            {
                CaptureMousePosition();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == NativeMethods.WM_HOTKEY && m.WParam.ToInt32() == NativeMethods.HOTKEY_ID_CAPTURE)
            {
                CaptureMousePosition();
                m.Result = IntPtr.Zero;
                return;
            }

            base.WndProc(ref m);
        }

        private void ActionEditDialog_Shown(object sender, EventArgs e)
        {
            NativeMethods.RegisterHotKey(this.Handle, NativeMethods.HOTKEY_ID_CAPTURE,
                NativeMethods.MOD_CONTROL | NativeMethods.MOD_SHIFT, (uint)Keys.Z);
        }

        private void ActionEditDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            NativeMethods.UnregisterHotKey(this.Handle, NativeMethods.HOTKEY_ID_CAPTURE);
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if ((keyData & Keys.Control) != 0 && (keyData & Keys.Shift) != 0 && (keyData & Keys.KeyCode) == Keys.Z)
            {
                CaptureMousePosition();
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }
    }
}
