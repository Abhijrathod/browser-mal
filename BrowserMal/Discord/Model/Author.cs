namespace BrowserMal.Discord.Model
{
    public class Author
    {
        public string name { get; set; }
        //public string url { get; set; }
        //public string icon_url { get; set; }

        public Author(string name)
        {
            this.name = name;
        }
    }
}
