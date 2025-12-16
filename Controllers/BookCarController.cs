using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using Utilities;
using TravelGroupProject.Models;

namespace TravelGroupProject.Controllers
{
    public class BookCarController : Controller
    {
        // GET - BookCar
        public IActionResult Index(string city, string state)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewData["City"] = city;
            ViewData["State"] = state;

            ViewBag.Cities = LoadCities();
            ViewBag.Classes = LoadClasses();

            List<Car> cars = LoadCars(city, null, null);

            return View("Index", cars);
        }

        // POST - Search
        [HttpPost]
        public IActionResult Search(string city, string selectedCity, string className, string keyword)
        {
            if (!string.IsNullOrEmpty(selectedCity))
            {
                city = selectedCity;
            }

            ViewData["City"] = city;
            ViewData["State"] = HttpContext.Request.Form["state"];

            ViewBag.Cities = LoadCities();
            ViewBag.Classes = LoadClasses();

            List<Car> cars = LoadCars(city, className, keyword);

            return View("Index", cars);
        }


        // POST - Add Car To My Vacation
        [HttpPost]
        public IActionResult AddCar(int carId, string companyName, string carInfo,
            int days, decimal pricePerDay)
        {
            decimal totalCost = days * pricePerDay;

            // Logged-in User
            if (HttpContext.Session.GetInt32("UserID") != null)
            {
                int userId = (int)HttpContext.Session.GetInt32("UserID");

                int bookingId = SaveBookingHeader(userId, totalCost);
                InsertCarBooking(bookingId, companyName, carInfo, days, pricePerDay, totalCost);
            }
            else
            {
                string json = HttpContext.Session.GetString("GuestCars");
                List<CarBooking> guestCars;

                if (string.IsNullOrEmpty(json))
                {
                    guestCars = new List<CarBooking>();
                }
                else
                {
                    guestCars = JsonSerializer.Deserialize<List<CarBooking>>(json);
                }

                CarBooking cb = new CarBooking();
                cb.CompanyName = companyName;
                cb.CarName = carInfo;
                cb.Days = days;
                cb.PricePerDay = pricePerDay;
                cb.TotalAmount = totalCost;
                cb.PaymentStatus = "Unpaid";
                cb.BookedOn = DateTime.Now;

                guestCars.Add(cb);

                HttpContext.Session.SetString("GuestCars", JsonSerializer.Serialize(guestCars));
            }

            TempData["Message"] = companyName + " – " + carInfo + " added to My Vacation!";

            return RedirectToAction("Index");
        }


        //Helpers 

        private List<string> LoadCities()
        {
            List<string> list = new List<string>();
            DBConnect db = new DBConnect();

            SqlCommand cmd = new SqlCommand("SELECT DISTINCT Name FROM City", db.GetConnection());
            DataSet ds = db.GetDataSet(cmd);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                list.Add(row["Name"].ToString());
            }

            return list;
        }

        private List<string> LoadClasses()
        {
            List<string> list = new List<string>();
            DBConnect db = new DBConnect();

            SqlCommand cmd = new SqlCommand("SELECT Name FROM CarClass", db.GetConnection());
            DataSet ds = db.GetDataSet(cmd);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                list.Add(row["Name"].ToString());
            }

            return list;
        }

        private List<Car> LoadCars(string city, string className, string keyword)
        {
            List<Car> list = new List<Car>();
            DBConnect db = new DBConnect();

            SqlCommand cmd = new SqlCommand("TP_GetCarsByCityOrClass", db.GetConnection());
            cmd.CommandType = CommandType.StoredProcedure;

            if (string.IsNullOrEmpty(city))
                cmd.Parameters.AddWithValue("@City", DBNull.Value);
            else
                cmd.Parameters.AddWithValue("@City", city);

            if (string.IsNullOrEmpty(className))
                cmd.Parameters.AddWithValue("@ClassName", DBNull.Value);
            else
                cmd.Parameters.AddWithValue("@ClassName", className);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                string make = row["Make"].ToString();
                string model = row["Model"].ToString();

                if (!string.IsNullOrEmpty(keyword))
                {
                    string lower = keyword.ToLower();

                    if (!make.ToLower().Contains(lower) && !model.ToLower().Contains(lower))
                    {
                        continue;
                    }
                }

                Car car = new Car();
                car.CarID = Convert.ToInt32(row["CarID"]);
                car.CompanyName = row["CompanyName"].ToString();
                car.Make = make;
                car.Model = model;
                car.Year = Convert.ToInt32(row["Year"]);
                car.ClassName = row["ClassName"].ToString();
                car.BasePricePerDay = Convert.ToDecimal(row["BasePricePerDay"]);

                string img = row["ImageURL"].ToString().Replace("\\", "/");
                car.ImageURL = "/" + img.TrimStart('/');

                list.Add(car);
            }

            return list;
        }


        // Save Booking Header
        private int SaveBookingHeader(int userId, decimal totalAmount)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand("TP_SaveUserBooking", db.GetConnection());
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@UserID", userId);
            cmd.Parameters.AddWithValue("@TotalAmount", totalAmount);
            cmd.Parameters.AddWithValue("@PaymentStatus", "Unpaid");
            cmd.Parameters.AddWithValue("@BookedOn", DateTime.Now);

            SqlParameter output = new SqlParameter("@NewBookingID", SqlDbType.Int);
            output.Direction = ParameterDirection.Output;

            cmd.Parameters.Add(output);

            db.DoUpdateUsingCmdObj(cmd);

            if (output.Value == DBNull.Value || output.Value == null)
            {
                throw new Exception("Failed to retrieve BookingID from TP_SaveUserBooking.");
            }

            return Convert.ToInt32(output.Value);
        }


        private void InsertCarBooking(int bookingId, string companyName,
            string carName, int days, decimal pricePerDay, decimal totalCost)
        {
            DBConnect db = new DBConnect();

            SqlCommand cmd = new SqlCommand("TP_InsertCarBooking", db.GetConnection());
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@BookingID", bookingId);
            cmd.Parameters.AddWithValue("@CompanyName", companyName);
            cmd.Parameters.AddWithValue("@CarName", carName);
            cmd.Parameters.AddWithValue("@Days", days);
            cmd.Parameters.AddWithValue("@PricePerDay", pricePerDay);
            cmd.Parameters.AddWithValue("@TotalCost", totalCost);


            db.DoUpdateUsingCmdObj(cmd);
        }
    }
}
