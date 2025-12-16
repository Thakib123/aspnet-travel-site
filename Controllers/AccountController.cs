using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using TravelGroupProject.Models;
using Utilities;

namespace TravelGroupProject.Controllers
{
    public class AccountController : Controller
    {
        // GET - Register
        public IActionResult Register()
        {
            LoadSecurityQuestions();
            return View(new User());
        }

        private void LoadSecurityQuestions()
        {
            List<string> questions = new List<string>()
            {
                "What is the name of your first pet?",
                "What city were you born in?",
                "What is your mother’s maiden name?",
                "What was your childhood nickname?",
                "What is your favorite teacher’s name?"
            };

            ViewBag.SecurityQuestions = questions;
        }

        // POST - Register
        [HttpPost]
        public IActionResult Register(User theUser)
        {
            LoadSecurityQuestions();

            try
            {
                if (theUser != null)
                {
                    DBConnect objDB = new DBConnect();
                    SqlCommand command = new SqlCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "TP_RegisterUser";

                    command.Parameters.AddWithValue("@Email", theUser.Email ?? "");
                    command.Parameters.AddWithValue("@Username", theUser.Username ?? "");
                    command.Parameters.AddWithValue("@Password", theUser.Password ?? "");
                    command.Parameters.AddWithValue("@Phone", theUser.Phone ?? "");
                    command.Parameters.AddWithValue("@Address", theUser.Address ?? "");
                    command.Parameters.AddWithValue("@SecurityQuestion1", theUser.SecurityQuestion1 ?? "");
                    command.Parameters.AddWithValue("@SecurityAnswer1", theUser.SecurityAnswer1 ?? "");
                    command.Parameters.AddWithValue("@SecurityQuestion2", theUser.SecurityQuestion2 ?? "");
                    command.Parameters.AddWithValue("@SecurityAnswer2", theUser.SecurityAnswer2 ?? "");
                    command.Parameters.AddWithValue("@SecurityQuestion3", theUser.SecurityQuestion3 ?? "");
                    command.Parameters.AddWithValue("@SecurityAnswer3", theUser.SecurityAnswer3 ?? "");

                    int result = objDB.DoUpdate(command);

                    if (result > 0)
                    {
                        
                        Response.Cookies.Append("PendingUserEmail", theUser.Email, new CookieOptions
                        {
                            Expires = DateTime.Now.AddMinutes(10),
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Strict
                        });

                        
                        return RedirectToAction("VerifyAccount");
                    }
                    else
                    {
                        ViewData["Message"] = "Registration failed.";
                    }

                    /* ViewData["Message"] = result > 0
                         ? "Registration successful!"
                         : "Registration failed.";*/
                }
            }
            catch (Exception ex)
            {
                ViewData["Message"] = "EXCEPTION: " + ex.Message;
            }

            return View(theUser);
        }

        // GET - Login
        public IActionResult Login()
        {
            return View();
        }

        // POST - Login WITH CAPTCHA
        [HttpPost]
        public IActionResult Login(string email, string password, string captchaInput, string rememberMe)
        {
            // CAPTCHA CHECK
            string savedCaptcha = HttpContext.Session.GetString("CaptchaCode");

            if (savedCaptcha == null || captchaInput != savedCaptcha)
            {
                ViewData["Message"] = "Incorrect CAPTCHA.";
                return View();
            }

            DBConnect objDB = new DBConnect();
            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "TP_GetUserByEmail";
            command.Parameters.AddWithValue("@Email", email);

            DataSet ds = objDB.GetDataSet(command);

            if (ds.Tables[0].Rows.Count > 0)
            {
                string storedPassword = ds.Tables[0].Rows[0]["Password"].ToString();

                if (storedPassword == password)
                {
                    int userId = Convert.ToInt32(ds.Tables[0].Rows[0]["UserID"]);

                    HttpContext.Session.SetInt32("UserID", userId);
                    HttpContext.Session.SetString("Email", email);
                    HttpContext.Session.SetString("UserName", ds.Tables[0].Rows[0]["Username"].ToString());
                    HttpContext.Session.SetString("Phone", ds.Tables[0].Rows[0]["Phone"]?.ToString() ?? "");
                    HttpContext.Session.SetString("Address", ds.Tables[0].Rows[0]["Address"]?.ToString() ?? "");

                    HttpContext.Session.Remove("GuestVacation");
                    HttpContext.Session.Remove("GuestCars");
                    HttpContext.Session.Remove("GuestFlights");
                    HttpContext.Session.Remove("GuestEvents");

                    if (!string.IsNullOrEmpty(rememberMe))
                    {
                        
                        Response.Cookies.Append("UserEmail", email);

                        Response.Cookies.Append("Password", password, new CookieOptions
                        {
                            Expires = DateTimeOffset.Now.AddDays(30)
                        });

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {    
                        Response.Cookies.Delete("UserEmail");
                        Response.Cookies.Delete("Password");    
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            ViewData["Message"] = "Invalid email or password.";
            return View();
        }

        public IActionResult ViewAccount()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login");
            }

            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand("TP_GetUserById");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserID", userId.Value);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);

            if (ds.Tables[0].Rows.Count == 0)
                return NotFound();

            DataRow row = ds.Tables[0].Rows[0];

            AccountViewModel model = new AccountViewModel
            {
                UserID = userId.Value,
                Email = row["Email"].ToString(),
                Username = row["Username"].ToString(),
                Phone = row["Phone"]?.ToString(),
                Address = row["Address"]?.ToString(),
                SecurityQuestion1 = row["SecurityQuestion1"].ToString(),
                SecurityAnswer1 = row["SecurityAnswer1"].ToString(),
                SecurityQuestion2 = row["SecurityQuestion2"].ToString(),
                SecurityAnswer2 = row["SecurityAnswer2"].ToString(),
                SecurityQuestion3 = row["SecurityQuestion3"].ToString(),
                SecurityAnswer3 = row["SecurityAnswer3"].ToString()
            };

            model.Images = GetUserImages(userId.Value);

            return View(model);
        }

        [HttpPost]
        public IActionResult ViewAccount(AccountViewModel model)
        {
            DBConnect db = new DBConnect();

            SqlCommand cmd = new SqlCommand("TP_UpdateUserProfile");
            cmd.CommandType = CommandType.StoredProcedure;
           

            cmd.Parameters.AddWithValue("@UserID", model.UserID);
            cmd.Parameters.AddWithValue("@Username", model.Username ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Phone", model.Phone ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Address", model.Address ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@SecurityAnswer1", model.SecurityAnswer1 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@SecurityAnswer2", model.SecurityAnswer2 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@SecurityAnswer3", model.SecurityAnswer3 ?? (object)DBNull.Value);

            int rows = db.DoUpdateUsingCmdObj(cmd);

            TempData["SuccessMessage"] = rows > 0
                ? "Account updated successfully!"
                : "No changes were made.";

            return View(model);
        }

        // Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult UploadImage(int userId, string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                TempData["ErrorMessage"] = "Please enter a valid Image URL";
                return RedirectToAction("ViewAccount", new { userId });
            }

            SaveUserImage(userId, imageUrl);

            TempData["SuccessMessage"] = "Image uploaded successfully";
            return RedirectToAction("ViewAccount", new { userId });
        }

        private List<UserImage> GetUserImages(int userId)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand("TP_GetUserImages");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserID", userId);

            DataTable dt = db.GetDataSetUsingCmdObj(cmd).Tables[0];
            List<UserImage> images = new List<UserImage>();

            foreach (DataRow row in dt.Rows)
            {
                images.Add(new UserImage
                {
                    UserId = userId,
                    ImageUrl = row["ImageUrl"].ToString()
                });
            }

            return images;
        }

        private void SaveUserImage(int userId, string imageUrl)
        {
            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand("TP_AddUserImage");
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@UserID", userId);
            cmd.Parameters.AddWithValue("@ImageUrl", imageUrl);

            db.DoUpdateUsingCmdObj(cmd);
        }

       
        public IActionResult VerifyAccount()
        {
            string email = Request.Cookies["PendingUserEmail"];

            if (email == null)
            {
                ViewBag.Error = "Session expired. Please try again.";
                return RedirectToAction("Register");
            }

            SendTwoFactorCode(email);
            return View();
        }
        

        
        [HttpPost]
        public IActionResult VerifyAccount(string inputCode)
        {
            string savedCode = Request.Cookies["TwoFactorCode"];
            string expiration = Request.Cookies["TwoFactorExpiration"];
            string email = Request.Cookies["PendingUserEmail"];

            if (savedCode == null || expiration == null)
            {
                ViewBag.Error = "Verification code not found. Please log in again.";
                return View();
            }

            if (DateTime.Now > DateTime.Parse(expiration))
            {
                ViewBag.Error = "Your verification code has expired.";
                return View();
            }

            if (inputCode != savedCode)
            {
                ViewBag.Error = "Incorrect verification code.";
                return View();
            }

            Response.Cookies.Append("UserEmail", email);
            Response.Cookies.Delete("TwoFactorCode");
            Response.Cookies.Delete("TwoFactorExpiration");
            Response.Cookies.Delete("PendingUserEmail");

            return RedirectToAction("Login", "Account");
        }
        

        
        public void SendTwoFactorCode(string email)
        {
            string code = GenerateVerificationCode();

            CookieOptions options = new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(10),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            Response.Cookies.Append("TwoFactorCode", code, options);
            Response.Cookies.Append("TwoFactorExpiration", DateTime.Now.AddMinutes(10).ToString(), options);

            Email newMail = new Email();
            newMail.SendMail(
                email,
                "vacationvibes@temple.edu",
                "Your Verification Code",
                $"<h2>Your verification code:</h2><h1>{code}</h1>"
            );
        }
        

        
        public string GenerateVerificationCode()
        {
            Random rnd = new Random();
            return rnd.Next(100000, 999999).ToString();
        }
        
        

        // ------------------------------
        // FORGOT PASSWORD FLOW
        // ------------------------------

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(string email)
        {
            DBConnect db = new DBConnect();

            SqlCommand cmd = new SqlCommand("TP_GetUserByEmail", db.GetConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Email", email);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);

            if (ds.Tables[0].Rows.Count == 0)
            {
                ViewBag.Error = "No account found with that email.";
                return View();
            }

            DataRow row = ds.Tables[0].Rows[0];

            HttpContext.Session.SetString("ResetEmail", email);

            ViewBag.Q1 = row["SecurityQuestion1"].ToString();
            ViewBag.Q2 = row["SecurityQuestion2"].ToString();
            ViewBag.Q3 = row["SecurityQuestion3"].ToString();

            return View("VerifySecurityQuestions");
        }

        [HttpPost]
        public IActionResult VerifySecurityQuestions(string A1, string A2, string A3)
        {
            string email = HttpContext.Session.GetString("ResetEmail");

            if (email == null)
            {
                ViewBag.Error = "Session expired.";
                return View("ForgotPassword");
            }

            DBConnect db = new DBConnect();

            SqlCommand cmd = new SqlCommand("TP_GetUserByEmail", db.GetConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Email", email);

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);
            DataRow row = ds.Tables[0].Rows[0];

            if (A1 != row["SecurityAnswer1"].ToString()
             || A2 != row["SecurityAnswer2"].ToString()
             || A3 != row["SecurityAnswer3"].ToString())
            {
                ViewBag.Error = "One or more answers are incorrect.";
                ViewBag.Q1 = row["SecurityQuestion1"].ToString();
                ViewBag.Q2 = row["SecurityQuestion2"].ToString();
                ViewBag.Q3 = row["SecurityQuestion3"].ToString();
                return View("VerifySecurityQuestions");
            }

            ViewBag.Email = email;
            return View("ResetPassword");
        }

        [HttpPost]
        public IActionResult ResetPassword(string newPassword)
        {
            string email = HttpContext.Session.GetString("ResetEmail");

            if (email == null)
            {
                ViewBag.Error = "Session expired.";
                return View("ForgotPassword");
            }

            DBConnect db = new DBConnect();

            SqlCommand cmd = new SqlCommand("TP_UpdateUserPassword", db.GetConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@NewPassword", newPassword);

            int result = db.DoUpdateUsingCmdObj(cmd);

            if (result > 0)
            {
                HttpContext.Session.Remove("ResetEmail");
                ViewBag.Message = "Password updated.";
                return View("Login");
            }

            ViewBag.Error = "Error updating password.";
            return View();
        }
    }
}
