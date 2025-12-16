namespace TravelGroupProject.Models
{
    public class Venue
    {
        private int venueId;
        private int cityId;
        private string venueName;
        private string venueDescription;
        private string phoneNumber;
        private string address;
        private string email;
        private string imageUrl;
        private string primaryEventType;

        public int VenueId
        {
            get { return venueId; }
            set { venueId = value; }
        }

        public int CityId
        {
            get { return cityId; }
            set { cityId = value; }
        }

        public string VenueName
        {
            get { return venueName; }
            set { venueName = value; }
        }

        public string VenueDescription
        {
            get { return venueDescription; }
            set { venueDescription = value; }
        }

        public string PhoneNumber
        {
            get { return phoneNumber; }
            set { phoneNumber = value; }
        }

        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        public string ImageUrl
        {
            get { return imageUrl; }
            set { imageUrl = value; }
        }

        public string PrimaryEventType
        {
            get { return primaryEventType; }
            set { primaryEventType = value; }
        }
    }
}
