namespace TravelGroupProject.Models
{
    public class SeatReservation
    {
        private int travelSiteId;
        private string travelSiteApiToken;
        private string packageName;
        private DateTime startDate;
        private DateTime endDate;
        private CustomerInfo customer;
        private List<BookedSeat> seats;
        private int packageId;

        public int TravelSiteId
        {
            get { return travelSiteId; }
            set { travelSiteId = value; }
        }

        public string TravelSiteApiToken
        {
            get { return travelSiteApiToken; }
            set { travelSiteApiToken = value; }
        }

        public string PackageName
        {
            get { return packageName; }
            set { packageName = value; }
        }

        public DateTime StartDate
        {
            get { return startDate; }
            set { startDate = value; }
        }

        public DateTime EndDate
        {
            get { return endDate; }
            set { endDate = value; }
        }

        public CustomerInfo Customer
        {
            get { return customer; }
            set { customer = value; }
        }

        public List<BookedSeat> Seats
        {
            get { return seats; }
            set { seats = value; }
        }

        public int PackageId
        {
            get { return packageId; }
            set { packageId = value; }
        }
    }
}
