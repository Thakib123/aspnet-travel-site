namespace TravelGroupProject.Models
{
    public class HotelDetailsResponse
    {
        private Hotel hotel;
        private List<Amenity> amenities;

        public Hotel Hotel
        {
            get { return hotel; }
            set { hotel = value; }
        }

        public List<Amenity> Amenities
        {
            get { return amenities; }
            set { amenities = value; }
        }
    }
}
