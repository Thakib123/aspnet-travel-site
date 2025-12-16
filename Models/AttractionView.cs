namespace TravelGroupProject.Models
{
    public class AttractionView
    {
        private Venue venue;
        private Event attraction;
        public Venue Venue
        {
            get { return venue; }
            set { venue = value; }
        }

        public Event Attraction
        {
            get { return attraction; }
            set { attraction = value; }
        }

    }
}
