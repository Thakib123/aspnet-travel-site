namespace TravelGroupProject.Models
{
    public class Hotel
    {
        private int hotelid;
        private string name;
        private string city;
        private string state;
        private string address;
        private string phone;
        private string email;
        private string imageurl;
        private string description;

        private List<Amenity> amenities;

        public int HotelID
        {
            get { return hotelid; }
            set { hotelid = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
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

        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        public string Phone
        {
            get { return phone; }
            set { phone = value; }
        }

        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        public string ImageURL
        {
            get { return imageurl; }
            set { imageurl = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public List<Amenity> Amenities
        {
            get { return amenities; }
            set { amenities = value; }
        }
    }
}
