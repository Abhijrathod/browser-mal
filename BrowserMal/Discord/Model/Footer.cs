using System.Reflection;

namespace BrowserMal.Discord.Model
{
    [Obfuscation(ApplyToMembers = false)]
    public class Footer
    {
        public string text { get; set; }
        //public string icon_url { get; set; }

        public Footer(string text)
        {
            this.text = text;
        }
    }
}
