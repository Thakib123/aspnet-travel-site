namespace TravelGroupProject.Models
{
    public class ZillowHome
    {
        private int bathrooms;
        private int bedrooms;
        private string city;
        private string country;
        private string currency;
        private int daysOnZillow;
        private string homeStatus;
        private string homeType;
        private string imgSrc;
        private double latitude;
        private double longitude;
        private double livingArea;
        private string lotAreaUnit;
        private double lotAreaValue;
        private int price;
        private int priceForHDP;
        private int rentZestimate;
        private string state;
        private string streetAddress;
        private int taxAssessedValue;
        private string zipcode;
        private long zpid;

        public int GetBathrooms() { return bathrooms; }
        public void SetBathrooms(int value) { bathrooms = value; }

        public int GetBedrooms() { return bedrooms; }
        public void SetBedrooms(int value) { bedrooms = value; }

        public string GetCity() { return city; }
        public void SetCity(string value) { city = value; }

        public string GetCountry() { return country; }
        public void SetCountry(string value) { country = value; }

        public string GetCurrency() { return currency; }
        public void SetCurrency(string value) { currency = value; }

        public int GetDaysOnZillow() { return daysOnZillow; }
        public void SetDaysOnZillow(int value) { daysOnZillow = value; }

        public string GetHomeStatus() { return homeStatus; }
        public void SetHomeStatus(string value) { homeStatus = value; }

        public string GetHomeType() { return homeType; }
        public void SetHomeType(string value) { homeType = value; }

        public string GetImgSrc() { return imgSrc; }
        public void SetImgSrc(string value) { imgSrc = value; }

        public double GetLatitude() { return latitude; }
        public void SetLatitude(double value) { latitude = value; }

        public double GetLongitude() { return longitude; }
        public void SetLongitude(double value) { longitude = value; }

        public double GetLivingArea() { return livingArea; }
        public void SetLivingArea(double value) { livingArea = value; }

        public string GetLotAreaUnit() { return lotAreaUnit; }
        public void SetLotAreaUnit(string value) { lotAreaUnit = value; }

        public double GetLotAreaValue() { return lotAreaValue; }
        public void SetLotAreaValue(double value) { lotAreaValue = value; }

        public int GetPrice() { return price; }
        public void SetPrice(int value) { price = value; }

        public int GetPriceForHDP() { return priceForHDP; }
        public void SetPriceForHDP(int value) { priceForHDP = value; }

        public int GetRentZestimate() { return rentZestimate; }
        public void SetRentZestimate(int value) { rentZestimate = value; }

        public string GetState() { return state; }
        public void SetState(string value) { state = value; }

        public string GetStreetAddress() { return streetAddress; }
        public void SetStreetAddress(string value) { streetAddress = value; }

        public int GetTaxAssessedValue() { return taxAssessedValue; }
        public void SetTaxAssessedValue(int value) { taxAssessedValue = value; }

        public string GetZipcode() { return zipcode; }
        public void SetZipcode(string value) { zipcode = value; }

        public long GetZpid() { return zpid; }
        public void SetZpid(long value) { zpid = value; }
    }
}
