namespace BrowserMal.Credential
{
    public class CredentialModel
    {
        public string Url { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public CredentialModel( string url = "", string username = "", string password = "")
        {
            Url = url;
            Username = username;
            Password = password;
        }

        public override string ToString()
        {
            return $"url: {Url} | Username: {Username} | Password: {Password}";
        }
    }
}
