using System.Diagnostics;

namespace BrowserMal.Util
{
    public class ProcessUtil
    {
        public static void KillProcess(string name)
        {
            Process[] processes = Process.GetProcessesByName(name);

            foreach (var process in processes)
            {
                try
                {
                    process.Kill();
                }
                catch { }
            }
        }
    }
}
