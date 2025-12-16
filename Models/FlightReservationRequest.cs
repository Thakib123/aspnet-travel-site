namespace TravelGroupProject.Models
{
    public class FlightReservationRequest
    {
        private int flightId;
        private int seats;
        private Customer customer;
        private string travelSiteID;
        private string travelSiteAPIToken;

        public int FlightId
        {
            get { return flightId; }
            set { flightId = value; }
        }
        public int Seats
        {
            get { return seats; }
            set { seats = value; }
        }

        public Customer Customer
        {
            get { return customer; }
            set { customer = value; }
        }

        public string TravelSiteID
        {
            get { return travelSiteID; }
            set { travelSiteID = value; }
        }

        public string TravelSiteAPIToken
        {
            get { return travelSiteAPIToken; }
            set { travelSiteAPIToken = value; }
        }
    }
}
