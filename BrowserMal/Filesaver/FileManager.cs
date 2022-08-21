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

        public static void SaveBytes(string root, string folder, byte[] file)
        {
            string path = Path.Combine(root, folder);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            File.WriteAllBytes(Path.Combine(path, $"{Path.GetRandomFileName()}_out.zip"), file);
        }
    }
}
