namespace BrowserMal.Model
{
    public class ColumnModel
    {
        public delegate string FormattingFunction(string value);
        private readonly bool isEncrypted;
        private readonly string name;

        public ColumnModel(string name, bool isEncrypted)
        {
            this.name = name;
            this.isEncrypted = isEncrypted;
        }

        public bool IsEncrypted() => isEncrypted;
        public string GetName() => name;
    }
}