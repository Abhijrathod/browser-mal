using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace BrowserMal.Util
{
    public class Zip
    {
        public static byte[] ZipArchives(Dictionary<string, string> fileContents)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, false))
                {
                    foreach (var file in fileContents)
                    {
                        ZipArchiveEntry zipArchiveEntry = zipArchive.CreateEntry(file.Key);

                        using (var originalFileStream = new MemoryStream(Encoding.UTF8.GetBytes(file.Value)))
                        {
                            using (var zipEntryStream = zipArchiveEntry.Open())
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
