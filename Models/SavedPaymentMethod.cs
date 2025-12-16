namespace TravelGroupProject.Models
{
    public class SavedPaymentMethod
    {
        private int paymentID;
        private string displayText;
        private string maskedNumber;
        private string cardholderName;
        private string exp;

        public int PaymentID
        {
            get { return paymentID; }
            set { paymentID = value; }
        }

        public string DisplayText
        {
            get { return displayText; }
            set { displayText = value; }
        }

        public string MaskedNumber
        {
            get { return maskedNumber; }
            set { maskedNumber = value; }
        }

        public string CardholderName
        {
            get { return cardholderName; }
            set { cardholderName = value; }
        }

        public string Exp
        {
            get { return exp; }
            set { exp = value; }
        }
    }
}
