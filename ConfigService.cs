using System;
using System.Collections.Generic;
using System.IO;

namespace JiuGeKeyClick
{
    public static class ConfigService
    {
        private static string _iniPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini");

        public static string ReadIniValue(string section, string key, string defaultValue = "")
        {
            if (!File.Exists(_iniPath))
                return defaultValue;

            char[] buffer = new char[256];
            int length = NativeMethods.GetPrivateProfileString(section, key, defaultValue, buffer, buffer.Length, _iniPath);
            return new string(buffer, 0, length);
        }

        public static void WriteIniValue(string section, string key, string value)
        {
            string dir = Path.GetDirectoryName(_iniPath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            NativeMethods.WritePrivateProfileString(section, key, value, _iniPath);
        }

        public static void LoadConfig(out string startKey, out List<ActionItem> actions)
        {
            startKey = ReadIniValue("Settings", "StartKey", "F8");
            int count = int.TryParse(ReadIniValue("Actions", "Count", "0"), out int c) ? c : 0;
            actions = new List<ActionItem>();

            for (int i = 0; i < count; i++)
            {
                string typeStr = ReadIniValue($"Action{i}", "Type", "Keyboard");
                string key = ReadIniValue($"Action{i}", "Key", "");
                int delay = int.TryParse(ReadIniValue($"Action{i}", "Delay", "500"), out int d) ? d : 500;
                bool enabled = bool.TryParse(ReadIniValue($"Action{i}", "Enabled", "True"), out bool e) ? e : true;
                int repeatCount = int.TryParse(ReadIniValue($"Action{i}", "RepeatCount", "1"), out int rc) ? rc : 1;
                int preDelay = int.TryParse(ReadIniValue($"Action{i}", "PreDelay", "0"), out int pd) ? pd : 0;
                string comment = ReadIniValue($"Action{i}", "Comment", "");
                int mouseX = int.TryParse(ReadIniValue($"Action{i}", "MouseX", "0"), out int mx) ? mx : 0;
                int mouseY = int.TryParse(ReadIniValue($"Action{i}", "MouseY", "0"), out int my) ? my : 0;
                bool useMousePos = bool.TryParse(ReadIniValue($"Action{i}", "UseMousePos", "False"), out bool ump) ? ump : false;
                string group = ReadIniValue($"Action{i}", "Group", "");
                string loopModeStr = ReadIniValue($"Action{i}", "LoopMode", "Once");

                if (Enum.TryParse(typeStr, out ActionType type))
                {
                    ActionItem action = new ActionItem(type, key, delay, enabled);
                    action.RepeatCount = repeatCount;
                    action.PreDelay = preDelay;
                    action.Comment = comment;
                    action.MouseX = mouseX;
                    action.MouseY = mouseY;
                    action.UseMousePos = useMousePos;
                    action.Group = group;
                    Enum.TryParse(loopModeStr, out LoopMode loopMode);
                    action.LoopMode = loopMode;
                    actions.Add(action);
                }
            }
        }

        public static void SaveConfig(string startKey, List<ActionItem> actions)
        {
            try
            {
                WriteIniValue("Settings", "StartKey", startKey);
                int oldCount = int.TryParse(ReadIniValue("Actions", "Count", "0"), out int oc) ? oc : 0;
                
                WriteIniValue("Actions", "Count", actions.Count.ToString());

                for (int i = 0; i < actions.Count; i++)
                {
                    ActionItem action = actions[i];
                    WriteIniValue($"Action{i}", "Type", action.Type.ToString());
                    WriteIniValue($"Action{i}", "Key", action.Key ?? "");
                    WriteIniValue($"Action{i}", "Delay", action.Delay.ToString());
                    WriteIniValue($"Action{i}", "Enabled", action.Enabled.ToString());
                    WriteIniValue($"Action{i}", "RepeatCount", action.RepeatCount.ToString());
                    WriteIniValue($"Action{i}", "PreDelay", action.PreDelay.ToString());
                    WriteIniValue($"Action{i}", "Comment", action.Comment ?? "");
                    WriteIniValue($"Action{i}", "MouseX", action.MouseX.ToString());
                    WriteIniValue($"Action{i}", "MouseY", action.MouseY.ToString());
                    WriteIniValue($"Action{i}", "UseMousePos", action.UseMousePos.ToString());
                    WriteIniValue($"Action{i}", "Group", action.Group ?? "");
                    WriteIniValue($"Action{i}", "LoopMode", action.LoopMode.ToString());
                }

                for (int i = actions.Count; i < oldCount; i++)
                {
                    WriteIniValue($"Action{i}", null, null);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("保存配置失败: " + ex.Message, ex);
            }
        }

        public static bool ImportConfig(string filePath, out string errorMessage)
        {
            errorMessage = "";
            
            try
            {
                if (!File.Exists(filePath))
                {
                    errorMessage = "配置文件不存在";
                    return false;
                }

                string destDir = Path.GetDirectoryName(_iniPath);
                if (!Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }

                string content = File.ReadAllText(filePath);
                if (string.IsNullOrWhiteSpace(content))
                {
                    WriteIniValue("Actions", "Count", "0");
                    return true;
                }

                File.WriteAllText(_iniPath, content);
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = "导入失败: " + ex.Message;
                return false;
            }
        }

        public static string GetConfigFilePath()
        {
            return _iniPath;
        }

        // 将配置保存到指定文件（导出/备份）
        public static void SaveToFile(string filePath, string startKey, List<ActionItem> actions)
        {
            string dir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            NativeMethods.WritePrivateProfileString("Settings", "StartKey", startKey, filePath);
            NativeMethods.WritePrivateProfileString("Actions", "Count", actions.Count.ToString(), filePath);

            for (int i = 0; i < actions.Count; i++)
            {
                ActionItem a = actions[i];
                NativeMethods.WritePrivateProfileString($"Action{i}", "Type", a.Type.ToString(), filePath);
                NativeMethods.WritePrivateProfileString($"Action{i}", "Key", a.Key ?? "", filePath);
                NativeMethods.WritePrivateProfileString($"Action{i}", "Delay", a.Delay.ToString(), filePath);
                NativeMethods.WritePrivateProfileString($"Action{i}", "Enabled", a.Enabled.ToString(), filePath);
                NativeMethods.WritePrivateProfileString($"Action{i}", "RepeatCount", a.RepeatCount.ToString(), filePath);
                NativeMethods.WritePrivateProfileString($"Action{i}", "PreDelay", a.PreDelay.ToString(), filePath);
                NativeMethods.WritePrivateProfileString($"Action{i}", "Comment", a.Comment ?? "", filePath);
                NativeMethods.WritePrivateProfileString($"Action{i}", "MouseX", a.MouseX.ToString(), filePath);
                NativeMethods.WritePrivateProfileString($"Action{i}", "MouseY", a.MouseY.ToString(), filePath);
                NativeMethods.WritePrivateProfileString($"Action{i}", "UseMousePos", a.UseMousePos.ToString(), filePath);
                NativeMethods.WritePrivateProfileString($"Action{i}", "Group", a.Group ?? "", filePath);
                NativeMethods.WritePrivateProfileString($"Action{i}", "LoopMode", a.LoopMode.ToString(), filePath);
            }
        }

        // 从指定文件加载配置（不覆盖默认 config.ini）
        public static bool LoadFromFile(string filePath, out string startKey, out List<ActionItem> actions)
        {
            startKey = "F8";
            actions = new List<ActionItem>();

            if (!File.Exists(filePath))
                return false;

            char[] buf = new char[256];
            int len;

            len = NativeMethods.GetPrivateProfileString("Settings", "StartKey", "F8", buf, buf.Length, filePath);
            startKey = new string(buf, 0, len);

            len = NativeMethods.GetPrivateProfileString("Actions", "Count", "0", buf, buf.Length, filePath);
            int count = int.TryParse(new string(buf, 0, len), out int c) ? c : 0;

            for (int i = 0; i < count; i++)
            {
                len = NativeMethods.GetPrivateProfileString($"Action{i}", "Type", "Keyboard", buf, buf.Length, filePath);
                string typeStr = new string(buf, 0, len);

                len = NativeMethods.GetPrivateProfileString($"Action{i}", "Key", "", buf, buf.Length, filePath);
                string key = new string(buf, 0, len);

                len = NativeMethods.GetPrivateProfileString($"Action{i}", "Delay", "500", buf, buf.Length, filePath);
                int delay = int.TryParse(new string(buf, 0, len), out int d) ? d : 500;

                len = NativeMethods.GetPrivateProfileString($"Action{i}", "Enabled", "True", buf, buf.Length, filePath);
                bool enabled = bool.TryParse(new string(buf, 0, len), out bool en) ? en : true;

                len = NativeMethods.GetPrivateProfileString($"Action{i}", "RepeatCount", "1", buf, buf.Length, filePath);
                int repeatCount = int.TryParse(new string(buf, 0, len), out int rc) ? rc : 1;

                len = NativeMethods.GetPrivateProfileString($"Action{i}", "PreDelay", "0", buf, buf.Length, filePath);
                int preDelay = int.TryParse(new string(buf, 0, len), out int pd) ? pd : 0;

                len = NativeMethods.GetPrivateProfileString($"Action{i}", "Comment", "", buf, buf.Length, filePath);
                string comment = new string(buf, 0, len);

                len = NativeMethods.GetPrivateProfileString($"Action{i}", "MouseX", "0", buf, buf.Length, filePath);
                int mouseX = int.TryParse(new string(buf, 0, len), out int mx) ? mx : 0;

                len = NativeMethods.GetPrivateProfileString($"Action{i}", "MouseY", "0", buf, buf.Length, filePath);
                int mouseY = int.TryParse(new string(buf, 0, len), out int my) ? my : 0;

                len = NativeMethods.GetPrivateProfileString($"Action{i}", "UseMousePos", "False", buf, buf.Length, filePath);
                bool useMousePos = bool.TryParse(new string(buf, 0, len), out bool ump) ? ump : false;

                len = NativeMethods.GetPrivateProfileString($"Action{i}", "Group", "", buf, buf.Length, filePath);
                string group = new string(buf, 0, len);

                len = NativeMethods.GetPrivateProfileString($"Action{i}", "LoopMode", "Once", buf, buf.Length, filePath);
                string loopModeStr = new string(buf, 0, len);

                if (Enum.TryParse(typeStr, out ActionType type))
                {
                    ActionItem action = new ActionItem(type, key, delay, enabled);
                    action.RepeatCount = repeatCount;
                    action.PreDelay = preDelay;
                    action.Comment = comment;
                    action.MouseX = mouseX;
                    action.MouseY = mouseY;
                    action.UseMousePos = useMousePos;
                    action.Group = group;
                    Enum.TryParse(loopModeStr, out LoopMode loopMode);
                    action.LoopMode = loopMode;
                    actions.Add(action);
                }
            }
            return true;
        }
    }
}