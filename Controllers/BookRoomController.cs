using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Data;
using System.Data.SqlClient;
using Utilities;
using TravelGroupProject.Models;

namespace TravelGroupProject.Controllers
{
    public class BookRoomController : Controller
    {
        private string apiUrl =
            "https://cis-iis2.temple.edu/Fall2025/CIS3342_tuo46790/WebAPI/api/HotelService/";

        // GET — Load the Book Room Page
        public IActionResult Book(int hotelId, string hotelName, int roomId, string roomName, string city, string state)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewData["City"] = city;
            ViewData["State"] = state;

            BookRoom book = new BookRoom
            {
                HotelID = hotelId,
                HotelName = hotelName,
                RoomID = roomId,
                RoomName = roomName
            };

            return View("Book", book);
        }

        // POST — Confirm Room Booking
        [HttpPost]
        public IActionResult Confirm(
            int hotelId,
            int roomId,
            string hotelName,
            string roomName,
            string city,
            string state,
            string firstName,
            string lastName,
            string email,
            DateTime checkIn,
            DateTime checkOut)
        {
            ViewData["City"] = city;
            ViewData["State"] = state;

            BookRoom book = new BookRoom
            {
                HotelID = hotelId,
                HotelName = hotelName,
                RoomID = roomId,
                RoomName = roomName
            };

            try
            {
                if (checkOut <= checkIn)
                {
                    ViewData["Message"] = "Check-Out must be after Check-In.";
                    return View("Book", book);
                }

                var bookingRequest = new
                {
                    Hotel = new { HotelID = hotelId },
                    Room = new { RoomID = roomId },
                    Customer = new { Name = firstName + " " + lastName, Email = email },
                    TravelSiteID = "KibVacationSite",
                    TravelSiteAPIToken = "Kib2025Key"
                };

                string jsonBody = JsonSerializer.Serialize(bookingRequest);

                using (HttpClient client = new HttpClient())
                {
                    StringContent body =
                        new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    HttpResponseMessage response =
                        client.PostAsync(apiUrl + "Reserve", body).Result;

                    string apiResponse = response.Content.ReadAsStringAsync().Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        ViewData["Message"] = "API Error: " + apiResponse;
                        return View("Book", book);
                    }
                }

                // SAVE FOR LOGGED-IN USER

                if (HttpContext.Session.GetInt32("UserID") != null)
                {
                    int userId = (int)HttpContext.Session.GetInt32("UserID");

                    decimal pricePerNight = GetRoomPrice(roomId);
                    int nights = (checkOut - checkIn).Days;
                    decimal totalAmount = pricePerNight * nights;

                    int bookingId = SaveUserBooking(userId, totalAmount);

                    if (bookingId <= 0)
                    {
                        throw new Exception("BOOKING ID FAILED: " + bookingId);
                    }

                    InsertHotelBooking(
                        bookingId,
                        hotelName,
                        roomName,
                        checkIn,
                        checkOut,
                        nights,
                        pricePerNight,
                        totalAmount
                    );


                    ViewData["Message"] = "Reservation successful!";
                    return View("Book", book);   
                }

                // Save for Guest Session
                {
                    decimal price = GetRoomPrice(roomId);
                    int nights = (checkOut - checkIn).Days;
                    decimal totalAmount = price * nights;

                    GuestHotel gh = new GuestHotel();
                    gh.HotelName = hotelName;
                    gh.RoomName = roomName;
                    gh.CheckInDate = checkIn;
                    gh.CheckOutDate = checkOut;
                    gh.TotalAmount = totalAmount;
                    gh.PaymentStatus = "Unpaid";

                    // Load guest session
                    List<GuestHotel> guestList;

                    string json = HttpContext.Session.GetString("GuestVacation");
                    if (string.IsNullOrEmpty(json))
                    {
                        guestList = new List<GuestHotel>();
                    }
                    else
                    {
                        guestList = JsonSerializer.Deserialize<List<GuestHotel>>(json);
                    }

                    guestList.Add(gh);

                    HttpContext.Session.SetString(
                        "GuestVacation",
                        JsonSerializer.Serialize(guestList)
                    );

                    ViewData["Message"] = "Reservation successful!";
                    return View("Book", book);
                }
            }
            catch (Exception ex)
            {
                ViewData["Message"] = ex.Message;
                return View("Book", book);
            }
        }

        // Helper Methods
        private decimal GetRoomPrice(int roomId)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand("TP_GetRoomPrice", db.GetConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RoomID", roomId);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);
            return Convert.ToDecimal(ds.Tables[0].Rows[0]["BasePrice"]);
        }

        private int SaveUserBooking(int userId, decimal totalAmount)
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
            return Convert.ToInt32(output.Value);
        }


        private void InsertHotelBooking(
            int bookingId,
            string hotelName,
            string roomName,
            DateTime checkIn,
            DateTime checkOut,
            int nights,
            decimal pricePerNight,
            decimal totalCost)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand("TP_InsertHotelBooking", db.GetConnection());
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@BookingID", bookingId);
            cmd.Parameters.AddWithValue("@HotelName", hotelName);
            cmd.Parameters.AddWithValue("@RoomName", roomName);
            cmd.Parameters.AddWithValue("@CheckInDate", checkIn);
            cmd.Parameters.AddWithValue("@CheckOutDate", checkOut);
            cmd.Parameters.AddWithValue("@Nights", nights);
            cmd.Parameters.AddWithValue("@PricePerNight", pricePerNight);
            cmd.Parameters.AddWithValue("@TotalCost", totalCost);

            try
            {
                int result = db.DoUpdateUsingCmdObj(cmd);

                if (result <= 0)
                {
                    throw new Exception(
                        "InsertHotelBooking failed (result=" + result +
                        "). BookingID=" + bookingId +
                        ", HotelName=" + hotelName +
                        ", RoomName=" + roomName +
                        ", CheckIn=" + checkIn.ToShortDateString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception("SQL ERROR — " + ex.Message +
                                    " | STACK: " + ex.StackTrace);
            }
        }




    }
}
