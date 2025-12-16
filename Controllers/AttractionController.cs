using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Text.Json;
using TravelGroupProject.Models;
using Utilities;


namespace TravelGroupProject.Controllers
{
    public class AttractionController : Controller
    {
        string baseUrl = "https://cis-iis2.temple.edu/Fall2025/CIS3342_tup84860/WebAPI/api/";
        private DBConnect db = new DBConnect();
        public IActionResult SearchEvents()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            string city = HttpContext.Session.GetString("SearchCity") ?? "";
            string state = HttpContext.Session.GetString("SearchState") ?? "";

            ViewData["City"] = city;
            ViewData["State"] = state;

            LoadEventCarouselImages();
            return View(new List<Event>());
        }

        [HttpPost]
        public IActionResult SearchEvents(string city, string state, string eventType)
        {
            if (!string.IsNullOrEmpty(city))
            {
                HttpContext.Session.SetString("SearchCity", city);
            }    
            if (!string.IsNullOrEmpty(state))
            {
                HttpContext.Session.SetString("SearchState", state);
            }

            List<Event> filteredEvents = new List<Event>();
            List<int> venueIds = new List<int>();

            HttpClient client = new HttpClient();

            string venuesUrl = baseUrl + "Venue/GetActivityAgencies?city="
                               + WebUtility.UrlEncode(city)
                               + "&state="
                               + WebUtility.UrlEncode(state);

            HttpResponseMessage venueResponse = client.GetAsync(venuesUrl).Result;
            if (venueResponse.IsSuccessStatusCode)
            {
                string venueJson = venueResponse.Content.ReadAsStringAsync().Result;
                List<Venue> venues = JsonSerializer.Deserialize<List<Venue>>(venueJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                int i = 0;
                while (venues != null && i < venues.Count)
                {
                    venueIds.Add(venues[i].VenueId);
                    i++;
                }
            }
            else
            {
                ViewBag.ErrorMessage = "An Error has occcured within the API";
                return View(filteredEvents);
            }

            HttpResponseMessage eventResponse = client.GetAsync(baseUrl + "Event/GetEvents").Result;
            if (eventResponse.IsSuccessStatusCode)
            {
                string eventJson = eventResponse.Content.ReadAsStringAsync().Result;
                List<Event> allEvents = JsonSerializer.Deserialize<List<Event>>(eventJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                int j = 0;
                while (allEvents != null && j < allEvents.Count)
                {
                    int k = 0;
                    while (k < venueIds.Count)
                    {
                        bool venueMatch = allEvents[j].VenueID == venueIds[k];
                        bool typeMatch = string.IsNullOrWhiteSpace(eventType)
                                         || allEvents[j].EventType.Equals(eventType, StringComparison.OrdinalIgnoreCase);

                        if (venueMatch && typeMatch)
                        {
                            filteredEvents.Add(allEvents[j]);
                            k = venueIds.Count;
                        }
                        k++;
                    }
                    j++;
                }
            }
            else
            {
                ViewBag.ErrorMessage = "There was an error with the API";
            }
            if (filteredEvents.Count == 0)
            {
                ViewBag.Message = "No events found matching your search.";
            }
            LoadEventCarouselImagesByType(eventType);
            return View(filteredEvents);
        }

        public IActionResult ViewEvent(int eventId, int venueId)
        {
            Event attraction = GetEventById(eventId);
            Venue venue = GetVenueById(venueId);

            var model = new AttractionView
            {
                Attraction = attraction,
                Venue = venue,
            };

            return View(model);

        }

        [HttpPost]
        public IActionResult ReserveTickets(int eventId, int venueId, int numberOfTickets)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                SaveGuestBooking(eventId, venueId, numberOfTickets);

                TempData["Message"] = "Tickets Saved Into Session Log In to Book!";
                return RedirectToAction("ViewEvent", new { eventId, venueId });
            }

            try
            {
                HttpClient client = new HttpClient();

                string requestUrl = $"{baseUrl}Event/UpdateAvailableTickets" +
                            $"?eventId={eventId}&ticketsToSubtract={numberOfTickets}";

                HttpResponseMessage response = client.PostAsync(requestUrl, null).Result;
                string jsonResponse = response.Content.ReadAsStringAsync().Result;

                var result = JsonSerializer.Deserialize<EventResponse>(
            jsonResponse,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (response.IsSuccessStatusCode && result != null && result.Success)
                {
                    decimal ticketPrice = GetTicketPrice(eventId);
                    int bookingId = SaveLocalBooking(userId.Value,
                                                     eventId,
                                                     ticketPrice * numberOfTickets);

                    AddAttractionBooking(bookingId, eventId, venueId, ticketPrice, numberOfTickets);

                    TempData["Message"] = "Tickets reserved successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update ticket availability.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
                TempData["ErrorMessage"] = "Error: " + ex.Message;
            }

            return RedirectToAction("ViewEvent", new { eventId, venueId });
        }

        private Event GetEventById(int eventId)
        {
            HttpClient client = new HttpClient();
            var response = client.GetAsync($"{baseUrl}Event/GetEventById?eventId={eventId}").Result;

            string json = response.Content.ReadAsStringAsync().Result;
            return JsonSerializer.Deserialize<Event>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        private Venue GetVenueById(int venueId)
        {
            HttpClient client = new HttpClient();
            var response = client.GetAsync($"{baseUrl}Venue/GetVenueById?venueId={venueId}").Result;

            string json = response.Content.ReadAsStringAsync().Result;
            return JsonSerializer.Deserialize<Venue>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }


        private void LoadEventCarouselImages()
        {
            SqlCommand cmd = new SqlCommand("TP_ShowEventImages");
            cmd.CommandType = CommandType.StoredProcedure;

            DataSet ds = db.GetDataSet(cmd);

            List<string> images = new List<string>();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                images.Add(row["EventImageUrl"].ToString());
            }

            ViewBag.CarouselImages = images;
        }

        private void LoadEventCarouselImagesByType(string eventType)
        {
            SqlCommand cmd = new SqlCommand("TP_ShowEventImagesByType");
            cmd.CommandType = CommandType.StoredProcedure;

            if (string.IsNullOrEmpty(eventType))
                cmd.Parameters.AddWithValue("@EventType", DBNull.Value);
            else
                cmd.Parameters.AddWithValue("@EventType", eventType);

            DataSet ds = db.GetDataSet(cmd);

            List<string> images = new List<string>();
            foreach (DataRow row in ds.Tables[0].Rows)
                images.Add(row["EventImageUrl"].ToString());

            ViewBag.CarouselImages = images;

        }

        private string GetEventName(int eventId)
        {
            HttpClient client = new HttpClient();
            string url = $"{baseUrl}Event/GetEventById?eventId={eventId}";

            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                Event ev = JsonSerializer.Deserialize<Event>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return ev?.EventName ?? "Unknown Event";
            }

            return "Unknown Event";
        }

        private decimal GetTicketPrice(int eventId)
        {
            HttpClient client = new HttpClient();
            string url = $"{baseUrl}Event/GetEventById?eventId={eventId}";

            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                Event ev = JsonSerializer.Deserialize<Event>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return ev?.TicketPrice ?? 0;
            }

            return 0;
        }



        private int SaveLocalBooking(int userId, int packageId, decimal totalAmount)
        {
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

        private void AddAttractionBooking(int bookingId, int eventId, int venueId, decimal eventPrice, int numberOfTickets)
        {
            SqlCommand cmd = new SqlCommand("TP_AddAttractionBooking");
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@BookingID", bookingId);
            cmd.Parameters.AddWithValue("@EventID", eventId);
            cmd.Parameters.AddWithValue("@VenueID", venueId);
            cmd.Parameters.AddWithValue("@EventName", GetEventName(eventId));
            cmd.Parameters.AddWithValue("@EventPrice", eventPrice);
            cmd.Parameters.AddWithValue("@NumberOfTickets", numberOfTickets);

            db.DoUpdateUsingCmdObj(cmd);
        }


        private void SaveGuestBooking(int eventId, int venueId, int numberOfTickets)
        {
            if (numberOfTickets == null || numberOfTickets <= 0)
            {
                return;
            }
            string sessionData = HttpContext.Session.GetString("GuestEvents");

            List<GuestAttraction> guestList;
            if (string.IsNullOrEmpty(sessionData))
            {
                guestList = new List<GuestAttraction>();
            }
            else
            {
                guestList = JsonSerializer.Deserialize<List<GuestAttraction>>(sessionData,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            GuestAttraction existing = null;
            foreach (var g in guestList)
            {
                if (g.EventId == eventId)
                {
                    existing = g;
                    break;
                }
            }

            decimal ticketPrice = GetTicketPrice(eventId);
            string eventName = GetEventName(eventId);

            if (existing == null)
            {
                existing = new GuestAttraction
                {
                    EventId = eventId,
                    VenueId = venueId,
                    NumberOfTickets = numberOfTickets,
                    EventName = eventName,
                    TicketPrice = ticketPrice,
                    TotalCost = numberOfTickets * ticketPrice,
                    BookedOn = DateTime.Now,
                    PaymentStatus = "Unpaid"
                };
                guestList.Add(existing);
            }
            else
            {
                existing.NumberOfTickets += numberOfTickets;
                existing.TicketPrice = ticketPrice;
                existing.TotalCost = existing.NumberOfTickets * ticketPrice;
            }

            HttpContext.Session.SetString("GuestEvents", JsonSerializer.Serialize(guestList));

        }

    }
}

