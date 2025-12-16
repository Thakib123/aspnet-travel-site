namespace TravelGroupProject.Models
{
    public class Airport
    {
        private int airportId;
        private string airportCode;
        private string airportName;
        private string address;
        private string city;
        private string state;
        private string country;
        private string imageUrl;

        public int AirportId
        {
            get { return airportId; } 
            set {  airportId = value; }
        }

        public string AirportCode
        {
            get { return airportCode; }
            set { airportCode = value; }
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

        public string Country
        {
            get { return country; }
            set { country = value; }
        }

        public string ImageUrl
        {
            get { return imageUrl; }
            set {  imageUrl = value; }
        }

    }
}
