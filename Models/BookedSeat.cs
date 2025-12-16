namespace TravelGroupProject.Models
{
    public class BookedSeat
    {
        private int seatId;
        private int flightId;
        private string seatNumber;

        public int SeatId
        {
            get { return seatId; }
            set { seatId = value; }
        }

        public int FlightId
        {
            get { return flightId; }
            set { flightId = value; }
        }
        public string SeatNumber
        {
            get { return seatNumber; }
            set { seatNumber = value; }
        }
    }
}
