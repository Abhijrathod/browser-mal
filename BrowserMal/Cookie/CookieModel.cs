namespace BrowserMal.Cookie
{
    public class CookieModel
    {
        public string Host { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Value { get; set; }

        public CookieModel(string host = "", string name = "", string path = "", string value = "")
        {
            Host = host;
            Name = name;
            Path = path;
            Value = value;
        }

        public override string ToString()
        {
            return $"Host: {Host} | Name: {Name} | Value: {Value}";
        }
    }
}
