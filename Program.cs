using System;
using System.Threading;
using System.Windows.Forms;

namespace JiuGeKeyClick
{
    static class Program
    {
        private static Mutex mutex = new Mutex(true, @"Local\JiuGeKeyClick_Mutex");

        [STAThread]
        static void Main()
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                try
                {
                    Application.Run(new MainForm());
                }
                finally
                {
                    mutex.ReleaseMutex();
                    mutex.Dispose();
                }
            }
            else
            {
                IntPtr hWnd = NativeMethods.FindWindow(null, "九歌键鼠助手");
                if (hWnd != IntPtr.Zero)
                {
                    NativeMethods.SetForegroundWindow(hWnd);
                }
            }
        }
    }
}