using BrowserMal.Browser;
using BrowserMal.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BrowserMal.Manager
{
    public abstract class Manager<T>
    {
        private readonly string tableName;
        private readonly SqliteTableModel sqliteTableModel;
        private readonly Dictionary<string, string> resultList;

        public Manager(string tableName, SqliteTableModel sqliteTableModel)
        {
            this.tableName = tableName;
            this.sqliteTableModel = sqliteTableModel;
            this.resultList = new Dictionary<string, string>();
        }

        public List<string> GetAllProfiles(string root) => Directory.GetDirectories(root).ToList();

        public abstract Dictionary<string, string> Init(ref List<BrowserModel> browsers, string profileType);

        public T CreateInstanceOfType(object[] args) => (T)Activator.CreateInstance(typeof(T), args);

        public string GetTableName => tableName;

        public SqliteTableModel _sqliteTableModel => sqliteTableModel;

        public Dictionary<string, string> _resultList => resultList;
    }
}
