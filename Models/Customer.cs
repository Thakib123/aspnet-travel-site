namespace TravelGroupProject.Models
{
    public class Customer
    {
        private string firstName;
        private string lastName;
        private string email;
        private string paymentMethod;
        private string cardNumber;
        private string expDate;
        private string cvv;

        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }

        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        public string PaymentMethod
        {
            get { return paymentMethod; }
            set { paymentMethod = value; }
        }

        public string CardNumber
        {
            get { return cardNumber; }
            set { cardNumber = value; }
        }

        public string ExpDate
        {
            get { return expDate; }
            set { expDate = value; }
        }

        public string CVV
        {
            get { return cvv; }
            set { cvv = value; }
        }
    }
}
