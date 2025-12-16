using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using TravelGroupProject.Models;
using Utilities;
using System.Net.Http.Json;


namespace TravelGroupProject.Controllers
{
    public class FlightController : Controller
    {

        private string baseApiUrl = "https://cis-iis2.temple.edu/Fall2025/CIS3342_tup92982/WebApiTest/api/Flight/";

        public IActionResult SearchFlights()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            string arriveCity = HttpContext.Session.GetString("SearchCity") ?? "";
            string arriveState = HttpContext.Session.GetString("SearchState") ?? "";

            ViewData["ArriveCity"] = arriveCity;
            ViewData["ArriveState"] = arriveState;

            LoadAirlines();
            return View(new List<Flight>());
        }

        private void LoadAirlines()
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = client.GetAsync(baseApiUrl +"LoadAirLines").Result;
                if (response.IsSuccessStatusCode)
                {
                    string content = response.Content.ReadAsStringAsync().Result;

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    List<Airline> airlines = JsonSerializer.Deserialize<List<Airline>>(content, options);

                    ViewBag.Airlines = new List<Airline>();
                    if (airlines != null)
                    {
                        ViewBag.Airlines.Add(new Airline { AirlineName = "All Airlines" });
                        ViewBag.Airlines.AddRange(airlines);
                    }
                }
                else
                {
                    ViewBag.Airlines = new List<Airline>();
                    ViewBag.Message = "Error loading airlines from API.";
                }
            }
            catch
            {
                ViewBag.Airlines = new List<Airline>();
                ViewBag.Message = "Error loading airlines from API.";
            }
        }

        [HttpPost]
        public IActionResult SearchFlights(string departCity, string departState, string arriveCity, string arriveState, string amenities, string airline)
        {
            if (!string.IsNullOrEmpty(arriveCity))
            {
                HttpContext.Session.SetString("SearchCity", arriveCity);
            }
            if (!string.IsNullOrEmpty(arriveState))
            {
                HttpContext.Session.SetString("SearchState", arriveState);
            }

            List<Flight> filteredFlights = new List<Flight>();

            try
            {
                HttpClient client = new HttpClient();

                string url = $"{baseApiUrl}FindFlights?departCity={WebUtility.UrlEncode(departCity)}" +
                           $"&departState={WebUtility.UrlEncode(departState)}" +
                           $"&arriveCity={WebUtility.UrlEncode(arriveCity)}" +
                           $"&arriveState={WebUtility.UrlEncode(arriveState)}" +
                           $"&amenities={WebUtility.UrlEncode(amenities)}";

                HttpResponseMessage response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    string content = response.Content.ReadAsStringAsync().Result;
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    List<Flight> allFlights = JsonSerializer.Deserialize<List<Flight>>(content, options);

                    // filter by airline dropdown
                    if (allFlights != null)
                    {
                        foreach (Flight f in allFlights)
                        {
                            if (airline == "All Airlines" || (!string.IsNullOrEmpty(f.AirlineName) && f.AirlineName.Equals(airline, StringComparison.OrdinalIgnoreCase)))
                            {
                                filteredFlights.Add(f);
                            }
                        }
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Error loading flights from API.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error: " + ex.Message;
            }

            LoadAirlines();
            return View(filteredFlights);
        }//Flights 

        public IActionResult ViewSeats(int flightId, double flightPrice)
        {
            List<FlightSeat> seats = GetSeatsFromApi(flightId);

            if (seats == null || seats.Count == 0)
            {
                ViewBag.Message = "Error loading seats.";
            }

            ViewBag.FlightId = flightId;
            ViewBag.FlightPrice = flightPrice;

            return View(seats);
        }

        [HttpPost]
        public IActionResult ReserveSeats(int flightId, double flightPrice, List<String> selectedSeatsRaw)
        {
            List<SeatSelection> selectedSeats = new List<SeatSelection>();
            if (selectedSeatsRaw != null)
            {
                foreach (var s in selectedSeatsRaw)
                {
                    var parts = s.Split('|');
                    selectedSeats.Add(new SeatSelection
                    {
                        SeatId = int.Parse(parts[0]),
                        SeatNumber = parts[1]
                    });
                }
            }

            List<FlightSeat> seats = GetSeatsFromApi(flightId);
            ViewBag.FlightId = flightId;
            ViewBag.FlightPrice = flightPrice;

            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                SaveGuestSeats(flightId, flightPrice, selectedSeats);

                ViewBag.Message = "Seats saved as guest please log in to reserve seats.";
                return View("ViewSeats", seats);
            }

            if (selectedSeats == null || selectedSeats.Count == 0)
            {
                ViewBag.Message = "No seats selected.";
                return View("ViewSeats", seats);
            }

            try
            {
                string apiToken = "xKgO/Nrtc0a1KBTaInFpWg==";
                int travelSiteId = 21;
                /*
                if (string.IsNullOrEmpty(apiToken) || travelSiteId == 0)
                {
                    HttpClient tokenClient = new HttpClient();
                    TokenRequest tokenReq = new TokenRequest { SiteName = "https://cis-iis2.temple.edu/Fall2025/CIS3342_tup92982" };
                    HttpResponseMessage tokenResp = tokenClient.PostAsJsonAsync(baseApiUrl + "GenerateToken", tokenReq).Result;
                    if (!tokenResp.IsSuccessStatusCode)
                    {
                        ViewBag.Message = "Failed to generate API token.";
                        return View("ViewSeats", seats);
                    }
                    string tokenJson = tokenResp.Content.ReadAsStringAsync().Result;
                    TravelSiteResponse tokenData = JsonSerializer.Deserialize<TravelSiteResponse>(tokenJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (!tokenData.Success)
                    {
                        ViewBag.Message = "Failed to generate API token.";
                        return View("ViewSeats", seats);
                    }
                    apiToken = tokenData.TravelSite.ApiToken;
                    travelSiteId = tokenData.TravelSite.TravelSiteId;
                    HttpContext.Session.SetString("TravelSiteApiToken", apiToken);
                    HttpContext.Session.SetInt32("TravelSiteId", travelSiteId);
                }
                */

                List<BookedSeat> mappedSeats = new List<BookedSeat>();
                foreach (var s in selectedSeats)
                {
                    mappedSeats.Add(new BookedSeat
                    {
                        FlightId = flightId,
                        SeatId = s.SeatId,
                        SeatNumber = s.SeatNumber
                    });
                }

                SeatReservation request = new SeatReservation
                {
                    TravelSiteId = travelSiteId,
                    TravelSiteApiToken = apiToken,
                    PackageName = "Flight Reservation",
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(1),
                    Seats = mappedSeats,
                    Customer = new CustomerInfo
                    {
                        UserId = 0,
                        UserName = HttpContext.Session.GetString("UserName") ?? "",
                        FirstName = HttpContext.Session.GetString("UserName") ?? "",
                        LastName = "Unknown",
                        Email = HttpContext.Session.GetString("Email") ?? "",
                        UserAddress = HttpContext.Session.GetString("Address") ?? "",
                        PhoneNumber = HttpContext.Session.GetString("Phone") ?? ""
                    },
                    PackageId = 0
                };

                HttpClient client = new HttpClient();
                HttpResponseMessage resp = client.PostAsJsonAsync(baseApiUrl + "ReserveSeat", request).Result;

                if (!resp.IsSuccessStatusCode)
                {
                    ViewBag.Message = "Failed to reserve seats through API.";
                    return View("ViewSeats", seats);
                }

                string respJson = resp.Content.ReadAsStringAsync().Result;
                Console.WriteLine(respJson);
                ApiResponse apiResult = JsonSerializer.Deserialize<ApiResponse>(respJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (apiResult != null && apiResult.Success)
                {
                    int packageId = apiResult.PackageId;
                    int bookingId = SaveLocalBooking(userId.Value, packageId, selectedSeats.Count * flightPrice );
                    foreach (var seat in selectedSeats)
                    {
                        AddFlightSeatBooking(bookingId, flightId, seat.SeatId, seat.SeatNumber, flightPrice);
                    }
                    ViewBag.PackageId = apiResult.PackageId;
                    ViewBag.Message = "Seats reserved successfully! Your Reservation ID is " +apiResult.PackageId+"";
                }
                else
                {
                    ViewBag.Message = "Failed to reserve seats.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error: " + ex.Message;
            }

            seats = GetSeatsFromApi(flightId);
            return View("ViewSeats", seats);
        }


        private int SaveLocalBooking(int userId, int packageId, double totalAmount)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand("TP_SaveUserBooking", db.GetConnection());
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@UserID", userId);
            cmd.Parameters.AddWithValue("@TotalAmount", totalAmount);
            cmd.Parameters.AddWithValue("@PaymentStatus", "Unpaid");
            cmd.Parameters.AddWithValue("@PackageID", packageId);
            cmd.Parameters.AddWithValue("@BookedOn", DateTime.Now);

             SqlParameter param = new SqlParameter("@NewBookingID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(param);

            db.DoUpdateUsingCmdObj(cmd);

            return (int)param.Value;
        }

        private void AddFlightSeatBooking(int bookingId, int flightId, int seatId, string seatNumber, double seatPrice)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand("TP_AddFlightSeatBooking");
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@BookingID", bookingId);
            cmd.Parameters.AddWithValue("@FlightID", flightId);
            cmd.Parameters.AddWithValue("@SeatID", seatId);
            cmd.Parameters.AddWithValue("@SeatNumber", seatNumber);
            cmd.Parameters.AddWithValue("@SeatPrice", seatPrice);
            try
            {
                db.DoUpdateUsingCmdObj(cmd);
            }
            catch (Exception ex)
            {
                throw new Exception("SQL ERROR — " + ex.Message);
            }
        }

        private List<FlightSeat> GetSeatsFromApi(int flightId)
        {
            List<FlightSeat> seats = new List<FlightSeat>();
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = client
                    .GetAsync(baseApiUrl + $"GetSeatsByFlightId?flightId={flightId}")
                    .Result;

                if (response.IsSuccessStatusCode)
                {
                    string content = response.Content.ReadAsStringAsync().Result;
                    seats = JsonSerializer.Deserialize<List<FlightSeat>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
            }
            catch(Exception ex) 
            {
                throw new Exception("Web api Error" + ex.Message);
            }

            return seats;
        }

        private void SaveGuestSeats(int flightId, double flightPrice, List<SeatSelection> selectedSeats)
        {
            if (selectedSeats == null || selectedSeats.Count == 0)
                return;

            string sessionData = HttpContext.Session.GetString("GuestFlights");

            List<GuestFlightBooking> guestList;
            if (string.IsNullOrEmpty(sessionData))
            {
                guestList = new List<GuestFlightBooking>();

            }
            else
            {
                guestList = JsonSerializer.Deserialize<List<GuestFlightBooking>>(sessionData,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            GuestFlightBooking existing = null;
            foreach (var g in guestList)
            {
                if (g.FlightId == flightId)
                {
                    existing = g;
                    break;
                }
            }

            if (existing == null)
            {
                existing = new GuestFlightBooking
                {
                    FlightId = flightId,
                    SeatsList = new List<GuestSeat>(),
                    SeatPrice = flightPrice,
                    TotalCost = 0,
                    BookedOn = DateTime.Now
                };
                guestList.Add(existing);
            }

            foreach (var seat in selectedSeats)
            {
                existing.SeatsList.Add(new GuestSeat
                {
                    SeatId = seat.SeatId,
                    SeatNumber = seat.SeatNumber
                });

                existing.TotalCost += existing.SeatPrice;
            }

            HttpContext.Session.SetString("GuestFlights",
            JsonSerializer.Serialize(guestList));
        }

    }
}
