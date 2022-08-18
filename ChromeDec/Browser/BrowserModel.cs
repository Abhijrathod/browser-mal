namespace ChromeDec.Browser
{
    public class BrowserModel
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string ProcessName { get; set; }

        public BrowserModel(string name, string location, string processName)
        {
            Name = name;
            Location = location;
            ProcessName = processName;
        }
    }
}
