namespace TravelGroupProject.Models
{
    public class GuestSeat
    {
        private int seatId;
        private string seatNumber;
        

        public GuestSeat() { }

        public int SeatId
        {
            get { return seatId; }
            set { seatId = value; }
        }


        public string SeatNumber
        {
            get { return seatNumber; }
            set { seatNumber = value; }
        }

        
    }
}
