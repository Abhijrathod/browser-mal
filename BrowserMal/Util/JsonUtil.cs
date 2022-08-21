using Newtonsoft.Json;
using System.Collections.Generic;

namespace BrowserMal.Util
{
    public class JsonUtil
    {
        public static string GetJson<T>(List<T> list) => JsonConvert.SerializeObject(list, Formatting.Indented);
    }
}
