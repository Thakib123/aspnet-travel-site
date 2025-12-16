namespace TravelGroupProject.Models
{
    public class BookRoom
    {
        private int roomid;
        private int hotelid;
        private string hotelname;
        private string roomname;

        public int RoomID
        {
            get { return roomid; }
            set { roomid = value; }
        }

        public int HotelID
        {
            get { return hotelid; }
            set { hotelid = value; }
        }

        public string HotelName
        {
            get { return hotelname; }
            set { hotelname = value; }
        }

        public string RoomName
        {
            get { return roomname; }
            set { roomname = value; }
        }
    }
}
