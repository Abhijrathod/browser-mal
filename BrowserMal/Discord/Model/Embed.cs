using Newtonsoft.Json;
using System.Collections.Generic;

namespace BrowserMal.Discord.Model
{
    public class Embed
    {
        [JsonProperty("color")]
        public int Color { get; set; }

        [JsonProperty("author")]
        public Author Author { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
        //public Thumbnail thumbnail { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("fields")]
        public List<Field> Fields { get; set; }
        //public Image image { get; set; }

        [JsonProperty("footer")]
        public Footer Footer { get; set; }
    }
}
