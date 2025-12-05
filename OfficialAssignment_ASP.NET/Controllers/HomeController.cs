using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OfficialAssignment_ASP.NET.Models;
using OfficialAssignment_ASP.NET.Models.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace OfficialAssignment_ASP.NET.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DatabaseHelper _dbHelper;

        public HomeController(ILogger<HomeController> logger, DatabaseHelper dbHelper)
        {
            _logger = logger;
            _dbHelper = dbHelper;
        }

        public IActionResult Index()
        {
            List<Product> products = new List<Product>();
            string query = "SELECT TOP 12 * FROM Products ORDER BY CASE WHEN Discount > 0 THEN 0 ELSE 1 END, NEWID()"; // Ưu tiên giảm giá, sau đó random
            
            DataTable dt = _dbHelper.ExecuteQuery(query);
            foreach (DataRow row in dt.Rows)
            {
                products.Add(new Product
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString(),
                    Price = Convert.ToDecimal(row["Price"]),
                    Image = row["Image"].ToString(),
                    Color = row["Color"].ToString(),
                    Size = row["Size"].ToString(),
                    Description = row["Description"].ToString(),
                    CategoryId = Convert.ToInt32(row["CategoryId"]),
                    Discount = row["Discount"] != DBNull.Value ? Convert.ToInt32(row["Discount"]) : 0
                });
            }

            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
