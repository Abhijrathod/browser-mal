using System.Net;

namespace BrowserMal.Util
{
    public class Web
    {
        public static string Get(string url)
        {
            using (WebClient wc = new WebClient())
            {
                return wc.DownloadString(url);
            }
        }
    }
}
