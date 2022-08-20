using System.Collections.Generic;

namespace BrowserMal.Model
{
    public class SqliteTableModel
    {
        private List<ColumnModel> columns;

        public SqliteTableModel(List<ColumnModel> columns) 
        {
            columns = new List<ColumnModel>();
            this.columns = columns;
        }

        public List<ColumnModel> GetColumns() => columns;
    }
}
