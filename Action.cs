using System;

namespace JiuGeKeyClick
{
    public enum ActionType
    {
        Keyboard,
        MouseLeft,
        MouseRight,
        MouseMiddle
    }

    public enum LoopMode
    {
        Once,
        Repeat,
        UntilStopped
    }

    public class ActionItem
    {
        public ActionType Type { get; set; }
        public string Key { get; set; }
        public int Delay { get; set; }
        public bool Enabled { get; set; }
        public int RepeatCount { get; set; }
        public int PreDelay { get; set; }
        public string Comment { get; set; }
        public int MouseX { get; set; }
        public int MouseY { get; set; }
        public bool UseMousePos { get; set; }
        public string Group { get; set; }
        public LoopMode LoopMode { get; set; }

        public ActionItem()
        {
            Type = ActionType.Keyboard;
            Key = string.Empty;
            Delay = 500;
            Enabled = true;
            RepeatCount = 1;
            PreDelay = 0;
            Comment = string.Empty;
            MouseX = 0;
            MouseY = 0;
            UseMousePos = false;
            Group = string.Empty;
            LoopMode = LoopMode.Once;
        }

        public ActionItem(ActionType type, string key, int delay, bool enabled) : this()
        {
            Type = type;
            Key = key;
            Delay = delay;
            Enabled = enabled;
        }

        public ActionItem Clone()
        {
            return new ActionItem(Type, Key, Delay, Enabled)
            {
                RepeatCount = RepeatCount,
                PreDelay = PreDelay,
                Comment = Comment,
                MouseX = MouseX,
                MouseY = MouseY,
                UseMousePos = UseMousePos,
                Group = Group,
                LoopMode = LoopMode,
            };
        }

        public string TypeDisplayName
        {
            get
            {
                switch (Type)
                {
                    case ActionType.Keyboard: return "键盘按键";
                    case ActionType.MouseLeft: return "鼠标左键";
                    case ActionType.MouseRight: return "鼠标右键";
                    case ActionType.MouseMiddle: return "鼠标中键";
                    default: return "未知";
                }
            }
        }

        public string KeyDisplayName
        {
            get
            {
                if (Type == ActionType.Keyboard)
                {
                    return string.IsNullOrEmpty(Key) ? "(未设置)" : Key;
                }
                else
                {
                    switch (Type)
                    {
                        case ActionType.MouseLeft: return "左键单击";
                        case ActionType.MouseRight: return "右键单击";
                        case ActionType.MouseMiddle: return "中键单击";
                        default: return string.Empty;
                    }
                }
            }
        }

        public string LoopModeDisplayName
        {
            get
            {
                switch (LoopMode)
                {
                    case LoopMode.Once: return "单次";
                    case LoopMode.Repeat: return "循环";
                    case LoopMode.UntilStopped: return "直到停止";
                    default: return "单次";
                }
            }
        }

        public override string ToString()
        {
            return $"{TypeDisplayName}: {KeyDisplayName} ({Delay}ms)";
        }
    }
}