using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.IO;
using System.Text.Json;
using TravelGroupProject.Models;
using Utilities;

namespace TravelGroupProject.Controllers
{
    public class HotelsController : Controller
    {
        private string apiUrl = "https://cis-iis2.temple.edu/Fall2025/CIS3342_tuo46790/WebAPI/api/HotelService/";

        // GET Search Hotels
        public IActionResult SearchHotels()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            LoadDropdowns();

            string savedJson = HttpContext.Session.GetString("SearchResults");

            if (!string.IsNullOrEmpty(savedJson))
            {
                List<Hotel> savedHotels =
                    JsonSerializer.Deserialize<List<Hotel>>(savedJson);

                return View(savedHotels);
            }

            return View(new List<Hotel>());
        }

        // POST Search Hotels
        [HttpPost]
        public IActionResult SearchHotels(string city, string state)
        {
            LoadDropdowns();

            if (string.IsNullOrEmpty(city) || string.IsNullOrEmpty(state))
            {
                ViewData["Message"] = "Please select both city and state.";
                return View(new List<Hotel>());
            }

            string url = apiUrl + "GetHotelsByCityState/" + city + "/" + state;

            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();

            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            string json = reader.ReadToEnd();

            reader.Close();
            response.Close();

            List<Hotel> hotels =
                JsonSerializer.Deserialize<List<Hotel>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            HttpContext.Session.SetString("SearchCity", city);
            HttpContext.Session.SetString("SearchState", state);
            HttpContext.Session.SetString("SearchResults", json);

            if (hotels == null || hotels.Count == 0)
            {
                ViewData["Message"] = "No hotels found.";
                return View(new List<Hotel>());
            }

            return View(hotels);
        }

        // GET Hotel Details
        public IActionResult Details(int id)
        {
            try
            {
                DBConnect db = new DBConnect();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "TP_GetHotelDetails";
                cmd.Parameters.AddWithValue("@HotelID", id);

                DataSet ds = db.GetDataSet(cmd);

                DataRow row = ds.Tables[0].Rows[0];

                Hotel h = new Hotel();
                h.HotelID = id;
                h.Name = row["HotelName"].ToString();
                h.City = row["City"].ToString();
                h.State = row["State"].ToString();
                h.Description = row["Description"].ToString();
                h.ImageURL = row["ImageURL"].ToString();
                h.Address = row["Address"].ToString();
                h.Phone = row["Phone"].ToString();
                h.Email = row["Email"].ToString();

                List<Amenity> amenityList = new List<Amenity>();

                foreach (DataRow aRow in ds.Tables[1].Rows)
                {
                    Amenity a = new Amenity();
                    a.Name = aRow["AmenityName"].ToString();
                    a.Category = aRow["Category"].ToString();
                    amenityList.Add(a);
                }

                h.Amenities = amenityList;

                return View(h);
            }
            catch (Exception ex)
            {
                ViewData["Message"] = ex.Message;
                return View();
            }
        }

        private void LoadDropdowns()
        {
            ViewBag.CityList = new List<string>()
            {
                "Philadelphia",
                "Las Vegas",
                "Orlando"
            };

            ViewBag.StateList = new List<string>()
            {
                "PA",
                "NV",
                "FL"
            };
        }
    }
}
