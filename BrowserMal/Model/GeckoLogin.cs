namespace BrowserMal.Model
{
    public class GeckoLogin
    {
        public long nextId { get; set; }
        public GeckoLoginData[] logins { get; set; }
        public string[] disabledHosts { get; set; }
        public int version { get; set; }
    }
}
