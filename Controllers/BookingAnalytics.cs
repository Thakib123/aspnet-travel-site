using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ScottPlot;
using ScottPlot.TickGenerators.Financial;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using TravelGroupProject.Models;
using Utilities;


namespace TravelGroupProject.Controllers
{
    public class BookingAnalytics : Controller
    {
        private DBConnect db = new DBConnect();
        private string baseApiUrl = "https://cis-iis2.temple.edu/Fall2025/CIS3342_tup92982/WebApiTest/api/Flight/";


        private Flight GetFlightById(int flightId)
        {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = client
                    .GetAsync(baseApiUrl + $"GetFlightByFlightId?flightId={flightId}")
                    .Result;

               string json = response.Content.ReadAsStringAsync().Result;

                return JsonSerializer.Deserialize<Flight>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

        public IActionResult FlightBookingAnalytics()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (!userId.HasValue)
                return RedirectToAction("Login", "Account");

            var routeBookingCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            // Get flight booking count
            SqlCommand cmd = new SqlCommand("TP_GetFlightBookingCount");
            cmd.CommandType = CommandType.StoredProcedure;

            DataSet ds = db.GetDataSetUsingCmdObj(cmd);

            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                ViewBag.NoData = true;
                return View();
            }

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                int flightId = Convert.ToInt32(row["FlightID"]);
                int tickets = Convert.ToInt32(row["TicketsSold"]);

                Flight flight = GetFlightById(flightId);
                if (flight == null)
                    continue;

                string route = flight.ArriveCity + "," + flight.ArriveState;

                if (!routeBookingCounts.ContainsKey(route))
                    routeBookingCounts[route] = 0;

                routeBookingCounts[route] += tickets;
            }

            if (routeBookingCounts.Count == 0)
            {
                ViewBag.NoData = true;
                return View();
            }

            int count = routeBookingCounts.Count;
            double[] xs = new double[count];
            double[] ys = new double[count];
            string[] labels = new string[count];

            int index = 0;
            foreach (var kvp in routeBookingCounts)
            {
                xs[index] = index;
                ys[index] = kvp.Value;
                labels[index] = kvp.Key;
                index++;
            }

            
            Plot myPlot = new();
            myPlot.Add.Bars(xs, ys);
            myPlot.Title("Most Popular Destinations");
            myPlot.YLabel("Number of Visitors");
            myPlot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(xs, labels);
            myPlot.Axes.Margins(bottom: 0);

            
            string tempFolder = Path.Combine(Path.GetTempPath(), "charts");
            Directory.CreateDirectory(tempFolder);

            string filePath = Path.Combine(tempFolder, "FlightBookings.png");
            myPlot.SavePng(filePath, 800, 450);

            ViewBag.NoData = false;

            return View();
        }

        //Helper method that shows the chart image
        public IActionResult GetChart()
        {
            string tempFolder = Path.Combine(Path.GetTempPath(), "charts");
            string filePath = Path.Combine(tempFolder, "FlightBookings.png");

            if (!System.IO.File.Exists(filePath))
            {
                ViewBag.ErrorMessage = "Error File Path does not exist";
                return View();
            }

            byte[] bytes = System.IO.File.ReadAllBytes(filePath);
            return File(bytes, "image/png");
        }

    }
}
