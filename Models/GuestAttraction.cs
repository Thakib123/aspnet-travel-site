namespace TravelGroupProject.Models
{
    public class GuestAttraction
    {
        private int eventId;
        private int venueId;
        private int numberOfTickets;
        private string eventName;
        private decimal ticketPrice;
        private decimal totalCost;
        private DateTime bookedOn;
        private string paymentStatus;

        public GuestAttraction() { }

        public int EventId
        {
            get { return eventId; }
            set { eventId = value; }
        }

        public int VenueId
        {
            get { return venueId; }
            set { venueId = value; }
        }

        public int NumberOfTickets
        {
            get { return numberOfTickets; }
            set { numberOfTickets = value; }
        }

        public string EventName
        {
            get { return eventName; }
            set { eventName = value; }
        }

        public decimal TicketPrice
        {
            get { return ticketPrice; }
            set { ticketPrice = value; }
        }
        public decimal TotalCost
        {
            get { return totalCost; }
            set { totalCost = value; }
        }

        public DateTime BookedOn
        {
            get { return bookedOn; }
            set { bookedOn = value; }
        }

        public string PaymentStatus
        {
            get { return paymentStatus; }
            set { paymentStatus = value; }
        }

    }
}
