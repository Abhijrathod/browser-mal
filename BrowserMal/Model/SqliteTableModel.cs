using System.Collections.Generic;

namespace BrowserMal.Model
{
    public class SqliteTableModel
    {
        private readonly List<ColumnModel> columns;

        public SqliteTableModel(List<ColumnModel> columns) => this.columns = columns;

        public List<ColumnModel> GetColumns() => columns;
    }
}
