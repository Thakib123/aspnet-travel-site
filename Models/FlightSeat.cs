namespace TravelGroupProject.Models
{
    public class FlightSeat
    {
        private int seatId;
        private int flightId;
        private string seatNumber;
        private bool isbooked;
        private string seatClass;

        public FlightSeat() { }

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

        public bool IsBooked
        {
            get { return isbooked; }
            set { isbooked = value; }
        }

        public string SeatClass
        {
            get { return seatClass; }
            set { seatClass = value; }
        }

    }
}
