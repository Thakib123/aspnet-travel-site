namespace TravelGroupProject.Models
{
    public class AccountViewModel
    {
        private int? userID;
        private string email;
        private string username;
        private string password;
        private string phone;
        private string address;
        private string securityquestion1;
        private string securityanswer1;
        private string securityquestion2;
        private string securityanswer2;
        private string securityquestion3;
        private string securityanswer3;
        private List<UserImage> images;

        public int? UserID
        {
            get { return userID; }
            set { userID = value; }
        }

        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        public string Phone
        {
            get { return phone; }
            set { phone = value; }
        }

        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        public string SecurityQuestion1
        {
            get { return securityquestion1; }
            set { securityquestion1 = value; }
        }

        public string SecurityAnswer1
        {
            get { return securityanswer1; }
            set { securityanswer1 = value; }
        }

        public string SecurityQuestion2
        {
            get { return securityquestion2; }
            set { securityquestion2 = value; }
        }

        public string SecurityAnswer2
        {
            get { return securityanswer2; }
            set { securityanswer2 = value; }
        }

        public string SecurityQuestion3
        {
            get { return securityquestion3; }
            set { securityquestion3 = value; }
        }

        public string SecurityAnswer3
        {
            get { return securityanswer3; }
            set { securityanswer3 = value; }
        }

        public List<UserImage> Images
        {
            get { return images; }
            set { images = value; }
        }

    }
}

