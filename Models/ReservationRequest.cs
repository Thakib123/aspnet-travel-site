namespace TravelGroupProject.Models
{
    public class ReservationRequest
    {
        private Hotel hotel;
        private Room room;
        private Customer customer;
        private string travelSiteID;
        private string travelSiteAPIToken;

        public Hotel Hotel
        {
            get { return hotel; }
            set { hotel = value; }
        }

        public Room Room
        {
            get { return room; }
            set { room = value; }
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
