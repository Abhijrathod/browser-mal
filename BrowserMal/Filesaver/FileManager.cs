using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace BrowserMal.Filesaver
{
    public class FileManager
    {
        public static void Save<T>(List<T> list, string outputPath, string fileName)
        {
            string json = JsonConvert.SerializeObject(list, Formatting.Indented);
            string output = Path.Combine(outputPath, fileName);

            File.WriteAllText(output, json);
        }
    }
}
