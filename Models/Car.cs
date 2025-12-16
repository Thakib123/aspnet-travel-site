namespace TravelGroupProject.Models
{
    public class Car
    {
        private int carID;
        private string companyName;
        private string make;
        private string model;
        private int year;
        private string className;
        private decimal basePricePerDay;
        private string imageURL;

        public Car() { }

        public int CarID
        {
            get { return carID; }
            set { carID = value; }
        }

        public string CompanyName
        {
            get { return companyName; }
            set { companyName = value; }
        }

        public string Make
        {
            get { return make; }
            set { make = value; }
        }

        public string Model
        {
            get { return model; }
            set { model = value; }
        }

        public int Year
        {
            get { return year; }
            set { year = value; }
        }

        public string ClassName
        {
            get { return className; }
            set { className = value; }
        }

        public decimal BasePricePerDay
        {
            get { return basePricePerDay; }
            set { basePricePerDay = value; }
        }

        public string ImageURL
        {
            get { return imageURL; }
            set { imageURL = value; }
        }
    }
}
