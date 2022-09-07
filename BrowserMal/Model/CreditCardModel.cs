using System.Reflection;

namespace BrowserMal.Model
{
    [ObfuscationAttribute(Exclude = true, ApplyToMembers = true)]
    public class CreditCardModel
    {
        public string CardHolder { get; set; }
        public string ExpirationMonth { get; set; }
        public string ExpirationYear { get; set; }
        public string Nickname { get; set; }
        public string CardNumber { get; set; }

        public CreditCardModel(string cardHolder = "", string expirationMonth = "", string expirationYear = "", string nickname = "", string cardNumber = "")
        {
            CardHolder = cardHolder;
            ExpirationMonth = expirationMonth;
            ExpirationYear = expirationYear;
            Nickname = nickname;
            CardNumber = cardNumber;
        }
    }
}
