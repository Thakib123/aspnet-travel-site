namespace TravelGroupProject.Models
{
    public class Event
    {
        private int eventID;
        private int venueID;
        private int cityID;
        private string eventName;
        private DateTime eventDate;
        private int totalTickets;
        private int remainingTickets;
        private decimal ticketPrice;
        private string eventType;

        public Event()
        {

        }
        public int EventID
        {
            get { return eventID; }
            set { eventID = value; }
        }

        public int VenueID
        {
            get { return venueID; }
            set { venueID = value; }
        }
        public int CityID
        {
            get { return cityID; }
            set { cityID = value; }
        }

        public string EventName
        {
            get { return eventName; }
            set { eventName = value; }
        }

        public DateTime EventDate
        {
            get { return eventDate; }
            set { eventDate = value; }
        }

        public int TotalTickets
        {
            get { return totalTickets; }
            set { totalTickets = value; }
        }

        public int RemainingTickets
        {
            get { return remainingTickets; }
            set { remainingTickets = value; }
        }

        public decimal TicketPrice
        {
            get { return ticketPrice; }
            set { ticketPrice = value; }
        }

        public string EventType
        {
            get { return eventType; }
            set { eventType = value; }
        }

     }
}
