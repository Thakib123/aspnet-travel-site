using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using Microsoft.Extensions.Options;
using TravelGroupProject.Models;
using Utilities;

namespace TravelGroupProject.Controllers
{
    public class PaymentController : Controller
    {
        private EncryptionSettings settings;

        public PaymentController(IOptions<EncryptionSettings> encSettings)
        {
            settings = encSettings.Value;
        }

        // GET Payment Page
        public IActionResult Index()
        {
            Payment model = new Payment();

            decimal total = CalculateTotal();
            model.TotalAmount = total;

            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId != null)
            {
                List<SavedPaymentMethod> saved = LoadSavedMethods(userId.Value);
                model.SavedCards = saved;
            }

            return View("Payment", model);
        }

        // POST Pay
        [HttpPost]
        public IActionResult Pay(Payment model)
        {
            model.TotalAmount = CalculateTotal();

            if (string.IsNullOrEmpty(model.CardholderName) ||
                string.IsNullOrEmpty(model.CardNumber) ||
                string.IsNullOrEmpty(model.Expiration) ||
                string.IsNullOrEmpty(model.CVV))
            {
                model.Message = "Please fill in all fields.";
                return View("Payment", model);
            }

            int? userId = HttpContext.Session.GetInt32("UserID");

            if (userId != null)
            {
                MarkUserBookingsPaid(userId.Value);

                if (model.SaveCard)
                {
                    SavePaymentMethod(userId.Value, model);
                }
            }
            else
            {
                MarkGuestBookingsPaid();
            }


             SendConfirmationEmail(model); 

            model.Message = "Payment Successful! Confirmation email sent.";

            return View("Payment", model);
        }

        // GET and calculate total
        private decimal CalculateTotal()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            decimal total = 0;

            if (userId != null)
            {
                DBConnect db = new DBConnect();

                // HOTELS
                SqlCommand cmdHotels = new SqlCommand("TP_GetUserHotels", db.GetConnection());
                cmdHotels.CommandType = CommandType.StoredProcedure;
                cmdHotels.Parameters.AddWithValue("@UserID", userId);
                DataSet dsHotels = db.GetDataSetUsingCmdObj(cmdHotels);
                total += SumUnpaid(dsHotels);

                // CARS
                SqlCommand cmdCars = new SqlCommand("TP_GetUserCars", db.GetConnection());
                cmdCars.CommandType = CommandType.StoredProcedure;
                cmdCars.Parameters.AddWithValue("@UserID", userId);
                DataSet dsCars = db.GetDataSetUsingCmdObj(cmdCars);
                total += SumUnpaid(dsCars);

                // FLIGHTS
                SqlCommand cmdFlights = new SqlCommand("TP_GetUserFlightSeats", db.GetConnection());
                cmdFlights.CommandType = CommandType.StoredProcedure;
                cmdFlights.Parameters.AddWithValue("@UserID", userId);
                DataSet dsFlights = db.GetDataSetUsingCmdObj(cmdFlights);
                total += SumUnpaid(dsFlights);

                // EVENTS
                SqlCommand cmdEvents = new SqlCommand("TP_GetUserAttractions", db.GetConnection());
                cmdEvents.CommandType = CommandType.StoredProcedure;
                cmdEvents.Parameters.AddWithValue("@UserID", userId);
                DataSet dsEvents = db.GetDataSetUsingCmdObj(cmdEvents);
                total += SumUnpaid(dsEvents);

                // ZILLOW RENTALS
                SqlCommand cmdZillow = new SqlCommand("TP_GetUserZillowRentals", db.GetConnection());
                cmdZillow.CommandType = CommandType.StoredProcedure;
                cmdZillow.Parameters.AddWithValue("@UserID", userId);
                DataSet dsZillow = db.GetDataSetUsingCmdObj(cmdZillow);
                total += SumUnpaid(dsZillow);

                db.GetConnection().Close();
                return total;
            }

            total += SumGuest<GuestHotel>("GuestVacation", h => h.PaymentStatus != "Paid", h => h.TotalAmount);
            total += SumGuest<GuestCar>("GuestCars", c => c.PaymentStatus != "Paid", c => c.TotalAmount);
            total += SumGuest<GuestFlightBooking>("GuestFlights", f => true, f => (decimal)f.TotalCost);

            return total;
        }

        private decimal SumUnpaid(DataSet ds)
        {
            decimal sum = 0;

            if (ds.Tables.Count > 0)
            {
                DataTable t = ds.Tables[0];
                int i = 0;

                while (i < t.Rows.Count)
                {
                    if (t.Rows[i]["PaymentStatus"].ToString() != "Paid")
                    {
                        sum += Convert.ToDecimal(t.Rows[i]["TotalAmount"]);
                    }
                    i++;
                }
            }
            return sum;
        }

        private decimal SumGuest<T>(string sessionKey, Func<T, bool> unpaidCheck, Func<T, decimal> amountSelector)
        {
            string json = HttpContext.Session.GetString(sessionKey);
            decimal total = 0;

            if (!string.IsNullOrEmpty(json))
            {
                List<T> items = JsonSerializer.Deserialize<List<T>>(json);
                int i = 0;
                while (i < items.Count)
                {
                    if (unpaidCheck(items[i]))
                    {
                        total += amountSelector(items[i]);
                    }
                    i++;
                }
            }
            return total;
        }

        // POST - Load Saved Card
        [HttpPost]
        public IActionResult LoadSavedCard(Payment model)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");

            if (userId != null && model.SelectedPaymentId > 0)
            {
                DBConnect db = new DBConnect();
                SqlCommand cmd = new SqlCommand("TP_GetPaymentMethodById", db.GetConnection());
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PaymentID", model.SelectedPaymentId);

                DataSet ds = db.GetDataSetUsingCmdObj(cmd);

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow r = ds.Tables[0].Rows[0];

                    model.CardholderName = r["CardholderName"].ToString();

                    // decrypt full card number
                    string encrypted = r["CardNumberEncrypted"].ToString();
                    string fullCardNumber = EncryptionHelper.Decrypt(
                        encrypted,
                        settings.Key,
                        settings.IV
                    );

                    model.CardNumber = fullCardNumber;
                    model.Expiration = r["ExpMonth"] + "/" + r["ExpYear"];
                    model.CVV = "";
                }
            }

            ModelState.Clear();

            model.SavedCards = LoadSavedMethods(userId.Value);
            model.TotalAmount = CalculateTotal();

            return View("Payment", model);
        }

        private List<SavedPaymentMethod> LoadSavedMethods(int userId)
        {
            List<SavedPaymentMethod> list = new List<SavedPaymentMethod>();

            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand("TP_GetPaymentMethodsByUser", db.GetConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserID", userId);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);

            if (ds.Tables.Count > 0)
            {
                DataTable t = ds.Tables[0];
                int i = 0;

                while (i < t.Rows.Count)
                {
                    DataRow r = t.Rows[i];

                    SavedPaymentMethod sp = new SavedPaymentMethod();
                    sp.PaymentID = Convert.ToInt32(r["PaymentID"]);
                    sp.DisplayText = r["Type"] + " ending in " + r["CardLast4"];
                    sp.MaskedNumber = r["CardToken"].ToString();
                    sp.CardholderName = r["CardholderName"].ToString();
                    sp.Exp = r["ExpMonth"].ToString() + "/" + r["ExpYear"].ToString();

                    list.Add(sp);
                    i++;
                }
            }

            return list;
        }

        private void SavePaymentMethod(int userId, Payment model)
        {
            string cardNum = model.CardNumber;
            string last4 = cardNum.Substring(cardNum.Length - 4);

            string encryptedCard = EncryptionHelper.Encrypt(
                cardNum,
                settings.Key,
                settings.IV
            );

            string exp = model.Expiration;
            string[] parts = exp.Split('/');
            int expMonth = Convert.ToInt32(parts[0]);
            int expYear = 2000 + Convert.ToInt32(parts[1]);

            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand("TP_SavePaymentMethod", db.GetConnection());
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@UserID", userId);
            cmd.Parameters.AddWithValue("@Type", "Card");
            cmd.Parameters.AddWithValue("@CardLast4", last4);
            cmd.Parameters.AddWithValue("@CardToken", "**** **** **** " + last4);
            cmd.Parameters.AddWithValue("@CardNumberEncrypted", encryptedCard);
            cmd.Parameters.AddWithValue("@ExpMonth", expMonth);
            cmd.Parameters.AddWithValue("@ExpYear", expYear);
            cmd.Parameters.AddWithValue("@CardholderName", model.CardholderName);

            db.DoUpdateUsingCmdObj(cmd);
        }

        private void MarkUserBookingsPaid(int userId)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand("TP_MarkBookingsPaid", db.GetConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserID", userId);
            db.DoUpdateUsingCmdObj(cmd);
        }

        private void MarkGuestBookingsPaid()
        {
            HttpContext.Session.Remove("GuestVacation");
            HttpContext.Session.Remove("GuestCars");
            HttpContext.Session.Remove("GuestFlights");
            HttpContext.Session.Remove("GuestEvents");
        }

        private void SendConfirmationEmail(Payment model)
        {
            string email = model.Email;

            if (string.IsNullOrEmpty(email))
            {
                email = HttpContext.Session.GetString("Email");

                if (string.IsNullOrEmpty(email))
                    email = "noreply@vacationvibes.com";
            }

            string body =
                "<h2>Vacation Vibes Payment Confirmation</h2>" +
                "<p>Total Paid: " + model.TotalAmount.ToString("C") + "</p>" +
                "<p>Thank you for booking with us!</p>";

            Email mail = new Email();
            mail.SendMail(email, "vacationvibes@temple.edu", "Payment Confirmation", body);
        }
    }
}
