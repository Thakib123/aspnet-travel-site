namespace TravelGroupProject.Models
{
    public class TopHotelViewModel
    {
        private int hotelID;
        private string name;
        private string city;
        private string state;
        private string description;
        private string imageURL;

        public int HotelID
        {
            get { return hotelID; }
            set { hotelID = value; }
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

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public string ImageURL
        {
            get { return imageURL; }
            set { imageURL = value; }
        }
    }
}
