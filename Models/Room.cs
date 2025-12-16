namespace TravelGroupProject.Models
{
    public class Room
    {
        private int roomid;
        private string name;
        private string bedtype;
        private int maxguests;
        private string description;
        private decimal baseprice;
        private string imageurl;
        private int hotelid;
        private string hotelname;
        private string city;
        private string state;

        public int RoomID
        {
            get { return roomid; }
            set { roomid = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string BedType
        {
            get { return bedtype; }
            set { bedtype = value; }
        }

        public int MaxGuests
        {
            get { return maxguests; }
            set { maxguests = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public decimal BasePrice
        {
            get { return baseprice; }
            set { baseprice = value; }
        }

        public string ImageURL
        {
            get { return imageurl; }
            set { imageurl = value; }
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

        public string City
        {
            get { return city; }
            set { city = value; }
        }

        public string State
        {
            get { return state; }
            set { state = value; }
        }
    }
}
