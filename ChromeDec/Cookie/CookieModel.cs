namespace BrowserMal.Cookie
{
    public class CookieModel
    {
        public string Host { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return $"Host: {Host} | Name: {Name} | Value: {Value}";
        }
    }
}
