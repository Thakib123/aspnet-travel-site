namespace TravelGroupProject.Models
{
    public class SeatSelection
    {
        private int seatId;
        private string seatNumber;

        public int SeatId
        {
            get { return seatId; }
            set {  seatId = value; }

        }

        public string SeatNumber
        {
            get { return seatNumber; }
            set { seatNumber = value; }
        }
    }
}
