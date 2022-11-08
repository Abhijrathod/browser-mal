using System.Reflection;

namespace BrowserMal.Discord.Model
{
    [Obfuscation(ApplyToMembers = false)]
    public class Field
    {
        public string name { get; set; }
        public string value { get; set; }

        public Field(string name, string value)
        {
            this.name = name;
            this.value = value;
        }
    }
}
