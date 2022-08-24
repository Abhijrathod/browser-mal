using System;
using System.Collections.Generic;
using System.IO;

namespace BrowserMal.Browser
{
    public class GeckoBrowserManager
    {
        public static string ApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        private readonly List<BrowserModel> _browserModelsList;

        public GeckoBrowserManager() => _browserModelsList = new List<BrowserModel>();

        public void Init()
        {
            _browserModelsList.Add(new BrowserModel("Firefox Developer Edition", Path.Combine(ApplicationData, @"Mozilla\Firefox\Profiles"), "", "dev-edition-default"));
            _browserModelsList.Add(new BrowserModel("Waterfox", Path.Combine(ApplicationData, @"Waterfox\Profiles"), ""));
            _browserModelsList.Add(new BrowserModel("SeaMonkey", Path.Combine(ApplicationData, @"Mozilla\SeaMonkey\Profiles"), ""));
            _browserModelsList.Add(new BrowserModel("Mozilla Thunderbird", Path.Combine(ApplicationData, @"Thunderbird\Profiles"), ""));
            _browserModelsList.Add(new BrowserModel("Mozilla Firefox", Path.Combine(ApplicationData, @"Mozilla\Firefox\Profiles"), "", ".default-release"));
            _browserModelsList.Add(new BrowserModel("Firefox Nightly", Path.Combine(ApplicationData, @"Mozilla\Firefox\Profiles"), "", "nightly"));
        }

        public List<BrowserModel> GetBrowsers() => _browserModelsList;
    }
}
