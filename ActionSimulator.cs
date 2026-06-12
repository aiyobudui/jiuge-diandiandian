using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace JiuGeKeyClick
{
    public class ActionSimulator
    {
        private volatile bool _isRunning = false;
        private Thread _workerThread = null;
        private List<ActionItem> _actions = new List<ActionItem>();
        private object _lock = new object();

        public bool IsRunning => _isRunning;

        public event EventHandler StatusChanged;
        public event EventHandler BeforeAction;
        public event EventHandler AfterAction;

        public void SetActions(List<ActionItem> actions)
        {
            lock (_lock)
            {
                _actions = new List<ActionItem>(actions);
            }
        }

        public void Start()
        {
            if (_isRunning) return;

            _isRunning = true;
            OnStatusChanged();

            _workerThread = new Thread(WorkerLoop);
            _workerThread.IsBackground = true;
            _workerThread.Start();
        }

        public void Stop()
        {
            _isRunning = false;
            if (_workerThread != null && _workerThread.IsAlive)
            {
                _workerThread.Join(3000);
            }
            OnStatusChanged();
        }

        private void WorkerLoop()
        {
            while (_isRunning)
            {
                List<ActionItem> currentActions;
                lock (_lock)
                {
                    currentActions = new List<ActionItem>(_actions);
                }

                foreach (ActionItem action in currentActions)
                {
                    if (!_isRunning) break;
                    if (!action.Enabled) continue;

                    if (action.PreDelay > 0)
                    {
                        WaitWithCheck(action.PreDelay);
                        if (!_isRunning) break;
                    }

                    for (int i = 0; i < action.RepeatCount; i++)
                    {
                        if (!_isRunning) break;
                        ExecuteAction(action);
                        WaitWithCheck(action.Delay);
                    }
                }
            }
        }

        private void ExecuteAction(ActionItem action)
        {
            BeforeAction?.Invoke(this, EventArgs.Empty);
            
            switch (action.Type)
            {
                case ActionType.Keyboard:
                    SimulateKeyPress(action.Key);
                    break;
                case ActionType.MouseLeft:
                    SimulateMouseClick(NativeMethods.MOUSEEVENTF_LEFTDOWN, NativeMethods.MOUSEEVENTF_LEFTUP, action.MouseX, action.MouseY);
                    break;
                case ActionType.MouseRight:
                    SimulateMouseClick(NativeMethods.MOUSEEVENTF_RIGHTDOWN, NativeMethods.MOUSEEVENTF_RIGHTUP, action.MouseX, action.MouseY);
                    break;
                case ActionType.MouseMiddle:
                    SimulateMouseClick(NativeMethods.MOUSEEVENTF_MIDDLEDOWN, NativeMethods.MOUSEEVENTF_MIDDLEUP, action.MouseX, action.MouseY);
                    break;
            }
            
            AfterAction?.Invoke(this, EventArgs.Empty);
        }

        private void SimulateKeyPress(string keyName)
        {
            if (string.IsNullOrEmpty(keyName)) return;

            List<Keys> keys = ParseKeyCombination(keyName);
            if (keys.Count == 0) return;

            foreach (Keys key in keys)
            {
                byte keyCode = (byte)key;
                NativeMethods.keybd_event(keyCode, 0, NativeMethods.KEYEVENTF_KEYDOWN, UIntPtr.Zero);
            }

            Thread.Sleep(10);

            foreach (Keys key in keys)
            {
                byte keyCode = (byte)key;
                NativeMethods.keybd_event(keyCode, 0, NativeMethods.KEYEVENTF_KEYUP, UIntPtr.Zero);
            }
        }

        private List<Keys> ParseKeyCombination(string keyName)
        {
            List<Keys> keys = new List<Keys>();
            
            string[] parts = keyName.Split('+');
            foreach (string part in parts)
            {
                Keys key = ParseKey(part.Trim());
                if (key != Keys.None)
                {
                    keys.Add(key);
                }
            }
            
            return keys;
        }

        private void SimulateMouseClick(uint downFlag, uint upFlag, int x, int y)
        {
            NativeMethods.SetCursorPos(x, y);
            NativeMethods.mouse_event(downFlag, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(10);
            NativeMethods.mouse_event(upFlag, 0, 0, 0, UIntPtr.Zero);
        }

        private void WaitWithCheck(int milliseconds)
        {
            int elapsed = 0;
            while (elapsed < milliseconds && _isRunning)
            {
                int waitTime = Math.Min(50, milliseconds - elapsed);
                Thread.Sleep(waitTime);
                elapsed += waitTime;
            }
        }

        private Keys ParseKey(string keyName)
        {
            if (string.IsNullOrEmpty(keyName))
                return Keys.None;

            string upperKey = keyName.ToUpper();

            switch (upperKey)
            {
                case "SPACE": return Keys.Space;
                case "ENTER": return Keys.Enter;
                case "TAB": return Keys.Tab;
                case "BACKSPACE": return Keys.Back;
                case "DELETE": return Keys.Delete;
                case "INSERT": return Keys.Insert;
                case "HOME": return Keys.Home;
                case "END": return Keys.End;
                case "PAGEUP": return Keys.PageUp;
                case "PAGEDOWN": return Keys.PageDown;
                case "UP": return Keys.Up;
                case "DOWN": return Keys.Down;
                case "LEFT": return Keys.Left;
                case "RIGHT": return Keys.Right;
                case "ESCAPE": return Keys.Escape;
                case "CAPSLOCK": return Keys.Capital;
                case "NUMLOCK": return Keys.NumLock;
                case "SCROLLLOCK": return Keys.Scroll;
                case "PRINTSCREEN": return Keys.PrintScreen;
                case "PAUSE": return Keys.Pause;
                case "WIN": return Keys.LWin;
                case "SHIFT": return Keys.ShiftKey;
                case "CONTROL": return Keys.ControlKey;
                case "CTRL": return Keys.ControlKey;
                case "ALT": return Keys.Menu;
                case "0": return Keys.D0;
                case "1": return Keys.D1;
                case "2": return Keys.D2;
                case "3": return Keys.D3;
                case "4": return Keys.D4;
                case "5": return Keys.D5;
                case "6": return Keys.D6;
                case "7": return Keys.D7;
                case "8": return Keys.D8;
                case "9": return Keys.D9;
                case "NUMPAD0": return Keys.NumPad0;
                case "NUMPAD1": return Keys.NumPad1;
                case "NUMPAD2": return Keys.NumPad2;
                case "NUMPAD3": return Keys.NumPad3;
                case "NUMPAD4": return Keys.NumPad4;
                case "NUMPAD5": return Keys.NumPad5;
                case "NUMPAD6": return Keys.NumPad6;
                case "NUMPAD7": return Keys.NumPad7;
                case "NUMPAD8": return Keys.NumPad8;
                case "NUMPAD9": return Keys.NumPad9;
                case "NUMPAD*": return Keys.Multiply;
                case "NUMPAD+": return Keys.Add;
                case "NUMPAD-": return Keys.Subtract;
                case "NUMPAD.": return Keys.Decimal;
                case "NUMPAD/": return Keys.Divide;
                case "NUMPADENTER": return Keys.Enter;
                case "+": return Keys.Oemplus;
                case "-": return Keys.OemMinus;
                case "*": return Keys.Multiply;
                case "/": return Keys.Divide;
                case ".": return Keys.OemPeriod;
                case ",": return Keys.Oemcomma;
                case ";": return Keys.OemSemicolon;
                case ":": return Keys.OemSemicolon;
                case "'": return Keys.OemQuotes;
                case "\"": return Keys.OemQuotes;
                case "[": return Keys.OemOpenBrackets;
                case "]": return Keys.OemCloseBrackets;
                case "\\": return Keys.OemBackslash;
                case "`": return Keys.Oemtilde;
                case "~": return Keys.Oemtilde;
                case "!": return Keys.D1;
                case "@": return Keys.D2;
                case "#": return Keys.D3;
                case "$": return Keys.D4;
                case "%": return Keys.D5;
                case "^": return Keys.D6;
                case "&": return Keys.D7;
                case "(": return Keys.D9;
                case ")": return Keys.D0;
                default:
                    if (keyName.Length == 1)
                    {
                        char c = keyName.ToUpper()[0];
                        if (c >= 'A' && c <= 'Z') return (Keys)c;
                        if (c >= '0' && c <= '9') 
                        {
                            int num = c - '0';
                            return (Keys)(Keys.D0 + num);
                        }
                    }
                    return Keys.None;
            }
        }

        private void OnStatusChanged()
        {
            StatusChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
