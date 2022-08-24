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
        private bool isSqlite;
        private string fileName;

        public Manager(string tableName, SqliteTableModel sqliteTableModel)
        {
            this.tableName = tableName;
            this.sqliteTableModel = sqliteTableModel;
            this.resultList = new Dictionary<string, string>();
        }

        public void SetIsSqlite(bool isSqlite) => this.isSqlite = isSqlite;
        public void SetFileName(string fileName) => this.fileName = fileName;

        public string _fileName => this.fileName;

        public bool _isSqlite => isSqlite;

        public List<string> GetAllProfiles(string root) => Directory.GetDirectories(root).ToList();

        public abstract Dictionary<string, string> Init(ref List<BrowserModel> browsers, string profileType);

        public T CreateInstanceOfType(object[] args) => (T)Activator.CreateInstance(typeof(T), args);

        public string _tableName => tableName;

        public SqliteTableModel _sqliteTableModel => sqliteTableModel;

        public Dictionary<string, string> _resultList => resultList;
    }
}
