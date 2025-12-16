using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using TravelGroupProject.Models;
using Utilities;

namespace TravelGroupProject.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (!userId.HasValue)
                return RedirectToAction("Login", "Account");

            var hotels = LoadTopHotels();

            return View(hotels);
        }


        private List<TopHotelViewModel> LoadTopHotels()
        {
            List<TopHotelViewModel> list = new List<TopHotelViewModel>();

            DBConnect db = new DBConnect();
            SqlCommand cmd = new SqlCommand("TP_GetHotelsByCityState", db.GetConnection());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@theCity", "Philadelphia");
            cmd.Parameters.AddWithValue("@theState", "PA");

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);

            int count = 0;

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if (count >= 4) break;

                TopHotelViewModel h = new TopHotelViewModel();
                h.HotelID = Convert.ToInt32(row["HotelID"]);
                h.Name = row["Name"].ToString();
                h.City = row["City"].ToString();
                h.State = row["State"].ToString();
                h.Description = row["Description"].ToString();
                h.ImageURL = row["ImageURL"].ToString();

                list.Add(h);
                count++;
            }

            return list;
        }

    }
}
