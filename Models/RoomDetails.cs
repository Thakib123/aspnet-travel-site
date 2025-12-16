namespace TravelGroupProject.Models
{
    public class RoomDetails
    {
        private int roomid;
        private string roomname;
        private string bedtype;
        private int maxguests;
        private string description;
        private decimal baseprice;
        private string imageurl;
        private List<Amenity> amenities;

        public int RoomID
        {
            get { return roomid; }
            set { roomid = value; }
        }

        public string RoomName
        {
            get { return roomname; }
            set { roomname = value; }
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

        public List<Amenity> Amenities
        {
            get { return amenities; }
            set { amenities = value; }
        }
    }
}
