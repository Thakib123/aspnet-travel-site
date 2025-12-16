namespace TravelGroupProject.Models
{
    public class GuestFlightBooking
    {
        private int flightId;
        private List<GuestSeat> seatsList;
        private double seatPrice;
        private double totalCost;
        private DateTime bookedOn;

        public int FlightId { 
            get { return flightId; } 
            set { flightId = value; }
        }

        public List<GuestSeat> SeatsList
        {
            get { return seatsList; }
            set { seatsList = value; }
        }

        public double SeatPrice
        {
            get { return seatPrice; }
            set { seatPrice = value; }
        }
        
        public double TotalCost
        {
            get { return totalCost; }
            set { totalCost = value; }
        }

        public DateTime BookedOn
        {
            get { return bookedOn; }
            set { bookedOn = value; }
        }

    }
}
