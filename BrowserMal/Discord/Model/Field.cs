namespace BrowserMal.Discord.Model
{
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
