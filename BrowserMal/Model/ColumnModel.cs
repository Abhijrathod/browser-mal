using System;

namespace BrowserMal.Model
{
    public class ColumnModel
    {
        private readonly bool isEncrypted;
        private readonly string name;
        private readonly bool needsFormatting;
        private readonly Func<object, object> function;
        private readonly bool isImportant;

        public ColumnModel(string name, bool isEncrypted, bool needsFormatting, bool isImportant, Func<object, object> function = null)
        {
            this.name = name;
            this.isEncrypted = isEncrypted;
            this.needsFormatting = needsFormatting;
            this.isImportant = isImportant;
            this.function = function;
        }

        public bool IsImportant() => isImportant;

        public Func<object, object> GetFunction() => function;

        public object Format(object value) => function(value);

        public bool IsEncrypted() => isEncrypted;

        public string GetName() => name;

        public bool IsNeedsFormatting() => needsFormatting;
    }
}