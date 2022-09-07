using System.Reflection;

namespace BrowserMal.Model
{
    [ObfuscationAttribute(Exclude = true, ApplyToMembers = true)]
    public class AddressesModel
    {
        public string StreetName { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }

        public AddressesModel(string streetName = "", string city = "", string postalCode = "")
        {
            StreetName = streetName;
            City = city;
            PostalCode = postalCode;
        }
    }
}
