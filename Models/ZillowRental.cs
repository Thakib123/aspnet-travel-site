namespace TravelGroupProject.Models
{
    public class ZillowRental
    {
        private string address;
        private string city;
        private string state;
        private string imageUrl;
        private int days;
        private decimal rate;
        private decimal totalAmount;
        private string paymentStatus;

        public string GetAddress() { return address; }
        public void SetAddress(string value) { address = value; }

        public string GetCity() { return city; }
        public void SetCity(string value) { city = value; }

        public string GetState() { return state; }
        public void SetState(string value) { state = value; }

        public string GetImageUrl() { return imageUrl; }
        public void SetImageUrl(string value) { imageUrl = value; }

        public int GetDays() { return days; }
        public void SetDays(int value) { days = value; }

        public decimal GetRate() { return rate; }
        public void SetRate(decimal value) { rate = value; }

        public decimal GetTotalAmount() { return totalAmount; }
        public void SetTotalAmount(decimal value) { totalAmount = value; }

        public string GetPaymentStatus() { return paymentStatus; }
        public void SetPaymentStatus(string value) { paymentStatus = value; }
    }
}
