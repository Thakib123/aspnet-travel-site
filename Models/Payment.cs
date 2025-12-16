namespace TravelGroupProject.Models
{
    public class Payment
    {
        private decimal totalAmount;
        private string message;
        private string email;
        private string cardholderName;
        private string cardNumber;
        private string expiration;
        private string cvv;
        private bool saveCard;
        private int selectedPaymentId;
        private List<SavedPaymentMethod> savedCards;

        public Payment()
        {
            savedCards = new List<SavedPaymentMethod>();
        }



        public decimal TotalAmount
        {
            get { return totalAmount; }
            set { totalAmount = value; }
        }

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        public string CardholderName
        {
            get { return cardholderName; }
            set { cardholderName = value; }
        }

        public string CardNumber
        {
            get { return cardNumber; }
            set { cardNumber = value; }
        }

        public string Expiration
        {
            get { return expiration; }
            set { expiration = value; }
        }

        public string CVV
        {
            get { return cvv; }
            set { cvv = value; }
        }

        public bool SaveCard
        {
            get { return saveCard; }
            set { saveCard = value; }
        }

        public int SelectedPaymentId
        {
            get { return selectedPaymentId; }
            set { selectedPaymentId = value; }
        }

        public List<SavedPaymentMethod> SavedCards
        {
            get { return savedCards; }
            set { savedCards = value; }
        }
    }
}
