namespace TravelGroupProject.Models
{
    public class CustomerInfo
    {
        private int userId;
        private string userName;
        private string firstName;
        private string lastName;
        private string email;
        private string userAddress;
        private string phoneNumber;

        public int UserId
        {
            get { return userId; }
            set { userId = value; }
        }

        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

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

        public string UserAddress
        {
            get { return userAddress; }
            set { userAddress = value; }
        }

        public string PhoneNumber
        {
            get { return phoneNumber; }
            set { phoneNumber = value; }
        }
    }
}
