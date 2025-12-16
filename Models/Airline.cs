namespace TravelGroupProject.Models
{
    public class Airline
    {

        private int airlineId;
        private string airlineName;
        private string airportName;
        private string address;
        private string city;
        private string state;
        private string description;
        private string phoneNumber;
        private string imageUrl;

        public Airline() { }

        public int AirlineId
        {
            get { return airlineId; }
            set { airlineId = value; }
        }

        public string AirlineName
        {
            get { return airlineName; }
            set { airlineName = value; }
        }

        public string AirportName
        {
            get { return airportName; }
            set { airportName = value; }
        }

        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        public string City
        {
            get { return city; }
            set { city = value; }
        }

        public string State
        {
            get { return state; }
            set { state = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public string PhoneNumber
        {
            get { return phoneNumber; }
            set { phoneNumber = value; }
        }

        public string ImageUrl
        {
            get { return imageUrl; }
            set { imageUrl = value; }
        }
    }
}
