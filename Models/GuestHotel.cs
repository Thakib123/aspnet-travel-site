namespace TravelGroupProject.Models
{
    public class GuestHotel
    {
        private string hotelName;
        private string roomName;
        private DateTime checkInDate;
        private DateTime checkOutDate;
        private decimal totalAmount;
        private string paymentStatus;

        public string HotelName
        {
            get { return hotelName; }
            set { hotelName = value; }
        }

        public string RoomName
        {
            get { return roomName; }
            set { roomName = value; }
        }

        public DateTime CheckInDate
        {
            get { return checkInDate; }
            set { checkInDate = value; }
        }

        public DateTime CheckOutDate
        {
            get { return checkOutDate; }
            set { checkOutDate = value; }
        }

        public decimal TotalAmount
        {
            get { return totalAmount; }
            set { totalAmount = value; }
        }

        public string PaymentStatus
        {
            get { return paymentStatus; }
            set { paymentStatus = value; }
        }
    }
}
