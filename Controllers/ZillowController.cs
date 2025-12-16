using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Data.SqlClient;
using System.Data;
using TravelGroupProject.Models;
using Utilities;

namespace TravelGroupProject.Controllers
{
    public class ZillowController : Controller
    {
        //First Choice
        //private string apiKey = "0b85c5d32dmsh03c3755e4ee8e23p1887dfjsnb20183a01fd9";

        //New if free trial runs out
        private string apiKey = "11d4b4e957mshbffa150694b0aebp16c397jsnd90abeeb675e";

        private string apiHost = "zillow56.p.rapidapi.com";

        // GET Search Page
        [HttpGet]
        public IActionResult Search()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (!userId.HasValue)
                return RedirectToAction("Login", "Account");

            return View(new List<ZillowHome>());
        }

        // POST — CALL ZILLOW API
        [HttpPost]
        public async Task<IActionResult> Search(string city)
        {
            List<ZillowHome> homes = new List<ZillowHome>();

            if (string.IsNullOrEmpty(city))
                return View(homes);

            string encodedCity = city.Replace(" ", "%20");

            string url =
                "https://zillow56.p.rapidapi.com/search?location=" +
                encodedCity +
                "&output=json&status=forSale&sortSelection=priorityscore";

            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage();

            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri(url);
            request.Headers.Add("x-rapidapi-key", apiKey);
            request.Headers.Add("x-rapidapi-host", apiHost);

            HttpResponseMessage response = await client.SendAsync(request);
            string json = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(json) || json.StartsWith("<"))
            {
                ViewData["Message"] = "Zillow API is temporarily unavailable.";
                return View(new List<ZillowHome>());
            }

            JsonDocument doc = JsonDocument.Parse(json);
            JsonElement results;

            if (doc.RootElement.TryGetProperty("results", out results))
            {
                foreach (JsonElement item in results.EnumerateArray())
                {
                    ZillowHome home = new ZillowHome();
                    JsonElement elem;

                    if (item.TryGetProperty("streetAddress", out elem))
                        home.SetStreetAddress(elem.GetString());
                    else
                        home.SetStreetAddress("Unknown");

                    if (item.TryGetProperty("city", out elem))
                        home.SetCity(elem.GetString());
                    else
                        home.SetCity("Unknown");

                    if (item.TryGetProperty("state", out elem))
                        home.SetState(elem.GetString());
                    else
                        home.SetState("N/A");

                    if (item.TryGetProperty("bedrooms", out elem) && elem.ValueKind == JsonValueKind.Number)
                        home.SetBedrooms((int)elem.GetDouble());
                    else
                        home.SetBedrooms(0);

                    if (item.TryGetProperty("bathrooms", out elem) && elem.ValueKind == JsonValueKind.Number)
                        home.SetBathrooms((int)elem.GetDouble());
                    else
                        home.SetBathrooms(0);

                    if (item.TryGetProperty("livingArea", out elem) && elem.ValueKind == JsonValueKind.Number)
                        home.SetLivingArea(elem.GetDouble());
                    else
                        home.SetLivingArea(0);

                    if (item.TryGetProperty("imgSrc", out elem))
                        home.SetImgSrc(elem.GetString());
                    else
                        home.SetImgSrc("");

                    if (item.TryGetProperty("zipcode", out elem))
                        home.SetZipcode(elem.GetString());
                    else
                        home.SetZipcode("00000");

                    if (item.TryGetProperty("zpid", out elem) && elem.ValueKind == JsonValueKind.Number)
                        home.SetZpid(elem.GetInt64());
                    else
                        home.SetZpid(0);

                    if (item.TryGetProperty("price", out elem))
                    {
                        try
                        {
                            if (elem.ValueKind == JsonValueKind.Number)
                            {
                                home.SetPrice(elem.GetInt32());
                            }
                            else if (elem.ValueKind == JsonValueKind.String)
                            {
                                int parsed;
                                string raw = elem.GetString();

                                if (int.TryParse(raw, out parsed))
                                    home.SetPrice(parsed);
                                else
                                    home.SetPrice(0);
                            }
                            else
                            {
                                home.SetPrice(0);
                            }
                        }
                        catch
                        {
                            home.SetPrice(0);
                        }
                    }
                    else
                    {
                        home.SetPrice(0);
                    }

                    homes.Add(home);
                }
            }

            return View(homes);
        }

        // POST — SAVE RENTAL TO DATABASE
        [HttpPost]
        public IActionResult RentNow(string address, string city, string state, int days)
        {
            int userId = (int)HttpContext.Session.GetInt32("UserID");

            decimal rate = 500m;   
            decimal total = rate * days;

            DBConnect db = new DBConnect();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "TP_AddZillowRental";

            cmd.Parameters.AddWithValue("@UserID", userId);
            cmd.Parameters.AddWithValue("@Address", address);
            cmd.Parameters.AddWithValue("@City", city);
            cmd.Parameters.AddWithValue("@State", state);
            cmd.Parameters.AddWithValue("@DailyRate", rate);
            cmd.Parameters.AddWithValue("@Days", days);
            cmd.Parameters.AddWithValue("@TotalAmount", total);
            cmd.Parameters.AddWithValue("@PaymentStatus", "Unpaid");

            db.DoUpdateUsingCmdObj(cmd);

            return RedirectToAction("Index", "MyVacation");
        }
    }
}
