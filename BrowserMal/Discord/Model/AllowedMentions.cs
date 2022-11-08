using System.Collections.Generic;
using System.Reflection;

namespace BrowserMal.Discord.Model
{
    [Obfuscation(ApplyToMembers = false)]
    public class AllowedMentions
    {
        public List<string> parse { get; set; }
    }
}
