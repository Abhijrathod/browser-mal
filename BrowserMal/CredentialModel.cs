namespace BrowserMal
{
    public class CredentialModel
    {
        public string Url { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public override string ToString()
        {
            return $"url: {Url} | Username: {Username} | Password: {Password}";
        }
    }
}
