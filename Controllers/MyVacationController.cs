using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using TravelGroupProject.Models;
using Utilities;

namespace TravelGroupProject.Controllers
{
    public class MyVacationController : Controller
    {
        public IActionResult Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");

            if (!userId.HasValue)
                return RedirectToAction("Login", "Account");

            return LoadUserBookings(userId.Value);
        }

        public IActionResult ViewPaidVacations()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");

            if (!userId.HasValue)
                return RedirectToAction("Login", "Account");

            return LoadUserPaidBookings(userId.Value);
        }

        //     LOAD PAID BOOKINGS
        private IActionResult LoadUserPaidBookings(int userId)
        {
            DBConnect db = new DBConnect();

            DataTable hotels = FilterPaidTable(GetData(db, "TP_GetUserHotels", userId));
            DataTable cars = FilterPaidTable(GetData(db, "TP_GetUserCars", userId));
            DataTable flights = FilterPaidTable(GetData(db, "TP_GetUserFlightSeats", userId));
            DataTable attractions = FilterPaidTable(GetData(db, "TP_GetUserAttractions", userId));

            ViewBag.Hotels = hotels;
            ViewBag.Cars = cars;
            ViewBag.Flights = flights;
            ViewBag.Attractions = attractions;

          
            SqlCommand cmd = new SqlCommand("TP_GetUserZillowRentals", db.GetConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserID", userId);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);

            DataTable zillow = new DataTable();
            if (ds.Tables.Count > 0)
                zillow = ds.Tables[0];

            zillow = FilterPaidTable(zillow); 

            ViewBag.ZillowRentals = zillow;

          
            ViewBag.HasPaid =
                hotels.Rows.Count > 0 ||
                cars.Rows.Count > 0 ||
                flights.Rows.Count > 0 ||
                attractions.Rows.Count > 0 ||
                zillow.Rows.Count > 0;

            return View("ViewPaidVacations");
        }

        //   LOAD UNPAID BOOKINGS
        private IActionResult LoadUserBookings(int userId)
        {
            DBConnect db = new DBConnect();

            DataTable hotels = FilterUnpaidTable(GetData(db, "TP_GetUserHotels", userId));
            DataTable cars = FilterUnpaidTable(GetData(db, "TP_GetUserCars", userId));
            DataTable flights = FilterUnpaidTable(GetData(db, "TP_GetUserFlightSeats", userId));
            DataTable attractions = FilterUnpaidTable(GetData(db, "TP_GetUserAttractions", userId));

            ViewBag.Hotels = hotels;
            ViewBag.Cars = cars;
            ViewBag.Flights = flights;
            ViewBag.Attractions = attractions;

            decimal hotelTotal = SumTable(hotels);
            decimal carTotal = SumTable(cars);
            decimal flightsTotal = SumTable(flights);
            decimal attractionsTotal = SumTable(attractions);

            ViewBag.HotelTotal = hotelTotal;
            ViewBag.CarTotal = carTotal;
            ViewBag.flightsTotal = flightsTotal;
            ViewBag.attractionsTotal = attractionsTotal;

            ViewBag.HasUnpaid =
                hotels.Rows.Count > 0 ||
                cars.Rows.Count > 0 ||
                flights.Rows.Count > 0 ||
                attractions.Rows.Count > 0;


            SqlCommand cmd = new SqlCommand("TP_GetUserZillowRentals", db.GetConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserID", userId);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);
            DataTable zillow = FilterUnpaidTable(ds.Tables[0]);

            ViewBag.ZillowRentals = zillow;

            decimal zillowTotal = 0;
            foreach (DataRow zr in zillow.Rows)
                zillowTotal += Convert.ToDecimal(zr["TotalAmount"]);

            ViewBag.ZillowTotal = zillowTotal;

            if (zillow.Rows.Count > 0)
                ViewBag.HasUnpaid = true;

            return View("Index");
        }

       

        private decimal SumTable(DataTable dt)
        {
            decimal total = 0;
            foreach (DataRow r in dt.Rows)
                total += Convert.ToDecimal(r["TotalAmount"]);
            return total;
        }

        private DataTable GetData(DBConnect db, string sp, int userId)
        {
            SqlCommand cmd = new SqlCommand(sp, db.GetConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserID", userId);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);
            return ds.Tables.Count > 0 ? ds.Tables[0] : new DataTable();
        }

        private DataTable FilterUnpaidTable(DataTable dt)
        {
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
                if (dt.Rows[i]["PaymentStatus"].ToString() == "Paid")
                    dt.Rows.RemoveAt(i);

            return dt;
        }

        private DataTable FilterPaidTable(DataTable dt)
        {
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
                if (dt.Rows[i]["PaymentStatus"].ToString() == "Unpaid")
                    dt.Rows.RemoveAt(i);

            return dt;
        }

        public IActionResult PayAll()
        {
            return RedirectToAction("Index", "Payment");
        }
    }
}
