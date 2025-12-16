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
    public class RoomsController : Controller
    {
        private string webApiUrl =
            "https://cis-iis2.temple.edu/Fall2025/CIS3342_tuo46790/WebAPI/api/HotelService/";

        public IActionResult SearchRooms(int hotelId, string hotelName, string city, string state)
        {
            ViewData["HotelID"] = hotelId;
            ViewData["HotelName"] = hotelName;
            ViewData["City"] = city;
            ViewData["State"] = state;

            List<Room> rooms = new List<Room>();

            try
            {
                string url = webApiUrl + "GetRoomsByHotel/" + hotelId;
                WebRequest request = WebRequest.Create(url);
                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                string json = reader.ReadToEnd();
                reader.Close();
                response.Close();

                // Save session
                HttpContext.Session.SetString("RoomsJson", json);
                HttpContext.Session.SetInt32("Rooms_HotelID", hotelId);
                HttpContext.Session.SetString("Rooms_HotelName", hotelName ?? "");
                HttpContext.Session.SetString("Rooms_City", city ?? "");
                HttpContext.Session.SetString("Rooms_State", state ?? "");

                rooms = JsonSerializer.Deserialize<List<Room>>(json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }
            catch
            {
                ViewData["Message"] = "Error loading rooms.";
            }

            return View(rooms);
        }

        public IActionResult BackToRooms()
        {
            string json = HttpContext.Session.GetString("RoomsJson");

            if (string.IsNullOrEmpty(json))
            {
                return RedirectToAction("SearchHotels", "Hotels");
            }

            int hotelId = HttpContext.Session.GetInt32("Rooms_HotelID") ?? 0;
            string hotelName = HttpContext.Session.GetString("Rooms_HotelName");
            string city = HttpContext.Session.GetString("Rooms_City");
            string state = HttpContext.Session.GetString("Rooms_State");

            ViewData["HotelID"] = hotelId;
            ViewData["HotelName"] = hotelName;
            ViewData["City"] = city;
            ViewData["State"] = state;

            List<Room> rooms = JsonSerializer.Deserialize<List<Room>>(json);

            return View("SearchRooms", rooms);
        }

        // Details 
        public IActionResult Details(int roomId)
        {
            try
            {
                DBConnect db = new DBConnect();
                SqlCommand cmd = new SqlCommand("TP_GetRoomDetails", db.GetConnection());
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RoomID", roomId);

                DataSet ds = db.GetDataSetUsingCmdObj(cmd);

                if (ds.Tables.Count >= 2 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow r = ds.Tables[0].Rows[0];
                    RoomDetails model = new RoomDetails();

                    model.RoomID = roomId;
                    model.RoomName = r["RoomName"].ToString();
                    model.BedType = r["BedType"].ToString();
                    model.MaxGuests = Convert.ToInt32(r["MaxGuests"]);
                    model.Description = r["Description"].ToString();
                    model.BasePrice = Convert.ToDecimal(r["BasePrice"]);
                    model.ImageURL = r["ImageURL"].ToString();

                    model.Amenities = new List<Amenity>();
                    foreach (DataRow a in ds.Tables[1].Rows)
                    {
                        Amenity item = new Amenity();
                        item.Name = a["AmenityName"].ToString();
                        item.Category = a["Category"].ToString();
                        model.Amenities.Add(item);
                    }

                    return View(model);
                }

                ViewData["Message"] = "Room not found.";
                return View();
            }
            catch (Exception ex)
            {
                ViewData["Message"] = "Error: " + ex.Message;
                return View();
            }
        }
        private decimal GetRoomPriceFromAPI(int roomId)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = $"{webApiUrl}GetRoomPrice/{roomId}";
                    HttpResponseMessage response = client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string json = response.Content.ReadAsStringAsync().Result;

                        var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

                        if (data != null && data.ContainsKey("pricePerNight"))
                        {
                            return Convert.ToDecimal(data["pricePerNight"]);
                        }
                    }
                }
            }
            catch
            {
            }

            return 0;
        }

        //  Booking
        [HttpGet]
        public IActionResult Book(int roomId, int hotelId, string hotelName, string roomName, string city, string state)
        {
            BookRoom book = new BookRoom();
            book.RoomID = roomId;
            book.HotelID = hotelId;
            book.HotelName = hotelName;
            book.RoomName = roomName;

            ViewData["City"] = city;
            ViewData["State"] = state;

            return View(book);
        }

        [HttpPost]
        public IActionResult Book(int roomId, int hotelId, string firstName, string lastName,
                          string email, DateTime checkIn, DateTime checkOut)
        {
            if (checkOut <= checkIn)
            {
                ViewData["Message"] = "Check-Out must be after Check-In.";
                return View();
            }

            decimal price = GetRoomPriceFromAPI(roomId);
            int nights = (checkOut - checkIn).Days;
            decimal total = nights * price;

            try
            {
                DBConnect db = new DBConnect();
                SqlCommand cmd = new SqlCommand("TP_InsertHotelBooking", db.GetConnection());
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@RoomID", roomId);
                cmd.Parameters.AddWithValue("@HotelID", hotelId);
                cmd.Parameters.AddWithValue("@FirstName", firstName);
                cmd.Parameters.AddWithValue("@LastName", lastName);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@CheckIn", checkIn);
                cmd.Parameters.AddWithValue("@CheckOut", checkOut);
                cmd.Parameters.AddWithValue("@Nights", nights);
                cmd.Parameters.AddWithValue("@TotalAmount", total);

                db.DoUpdateUsingCmdObj(cmd);

                ViewData["Message"] = $"Room booked successfully! Total: {total:C}.";
            }
            catch (Exception ex)
            {
                ViewData["Message"] = "Error booking room: " + ex.Message;
            }

            return View();
        }
        public IActionResult RentalPrompt(int hotelId, string hotelName, string city, string state)
        {
            ViewData["HotelID"] = hotelId;
            ViewData["HotelName"] = hotelName;
            ViewData["City"] = city;
            ViewData["State"] = state;

            return View();
        }

    }
}
