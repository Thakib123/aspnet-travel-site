namespace TravelGroupProject.Models
{
    public class Flight
    {
        private int flightId;
        private int airLineId;
        private string departCity;
        private string departState;
        private string arriveCity;
        private string arriveState;
        private DateTime departDate;
        private DateTime arriveDate;
        private int totalSeats;
        private bool isAvailable;
        private string flightClass;
        private double flightPrice;
        private List<string> amenities = new List<string>();
        private string airlineName;
        private string airportName;
        private string airlineImageUrl;

        public Flight() { }

        public int FlightId
        {
            get { return flightId; }
            set { flightId = value; }
        }

        public int AirLineId
        {
            get { return airLineId; }
            set { airLineId = value; }
        }

        public string DepartCity
        {
            get { return departCity; }
            set { departCity = value; }
        }

        public string DepartState
        {
            get { return departState; }
            set { departState = value; }
        }

        public string ArriveCity
        {
            get { return arriveCity; }
            set { arriveCity = value; }
        }

        public string ArriveState
        {
            get { return arriveState; }
            set { arriveState = value; }
        }

        public DateTime DepartDate
        {
            get { return departDate; }
            set { departDate = value; }
        }

        public DateTime ArriveDate
        {
            get { return arriveDate; }
            set { arriveDate = value; }
        }

        public int TotalSeats
        {
            get { return totalSeats; }
            set { totalSeats = value; }
        }

        public bool IsAvailable
        {
            get { return isAvailable; }
            set { isAvailable = value; }
        }

        public string FlightClass
        {
            get { return flightClass; }
            set { flightClass = value; }
        }

        public double FlightPrice
        {
            get { return flightPrice; }
            set { flightPrice = value; }
        }

        public List<string> Amenities
        {
            get { return amenities; }
            set { amenities = value; }
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

        public string AirlineImageUrl
        {
            get { return airlineImageUrl; }
            set { airlineImageUrl = value; }
        }
    }
}
