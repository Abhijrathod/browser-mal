using System.Reflection;

namespace BrowserMal.Discord.Model
{
    [Obfuscation(ApplyToMembers = false)]
    public class Author
    {
        public string name { get; set; }
        //public string url { get; set; }
        public string icon_url { get; set; }

        public Author(string name, string icon_url = "")
        {
            this.name = name;
            this.icon_url = icon_url;
        }
    }
}
