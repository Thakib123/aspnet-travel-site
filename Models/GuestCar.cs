namespace TravelGroupProject.Models
{
    public class GuestCar
    {
        private string companyName;
        private string carName;
        private int days;
        private decimal pricePerDay;
        private decimal totalAmount;
        private string paymentStatus;
        private DateTime bookedOn;


        public string CompanyName
        {
            get { return companyName; }
            set { companyName = value; }
        }

        public string CarName
        {
            get { return carName; }
            set { carName = value; }
        }

        public int Days
        {
            get { return days; }
            set { days = value; }
        }

        public decimal PricePerDay
        {
            get { return pricePerDay; }
            set { pricePerDay = value; }
        }

        public decimal TotalAmount
        {
            get { return totalAmount; }
            set { totalAmount = value; }
        }

        public string PaymentStatus
        {
            get { return paymentStatus; }
            set { paymentStatus = value; }
        }
        public DateTime BookedOn
        {
            get { return bookedOn; }
            set { bookedOn = value; }
        }
    }
}
