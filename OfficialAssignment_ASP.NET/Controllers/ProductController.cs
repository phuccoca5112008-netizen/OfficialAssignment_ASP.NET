using Microsoft.AspNetCore.Mvc;
using OfficialAssignment_ASP.NET.Models;
using OfficialAssignment_ASP.NET.Models.DAL;
using System;
using System.Data;
using System.Data.SqlClient;

namespace OfficialAssignment_ASP.NET.Controllers
{
    public class ProductController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public ProductController(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public IActionResult Details(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            Product product = null;
            string query = @"
                SELECT p.*, c.Name as CategoryName 
                FROM Products p 
                LEFT JOIN Categories c ON p.CategoryId = c.Id 
                WHERE p.Id = @Id";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Id", id)
            };

            DataTable dt = _dbHelper.ExecuteQuery(query, parameters);
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                product = new Product
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString(),
                    Price = Convert.ToDecimal(row["Price"]),
                    Image = row["Image"].ToString(),
                    Color = row["Color"].ToString(),
                    Size = row["Size"].ToString(),
                    Description = row["Description"].ToString(),
                    CategoryId = Convert.ToInt32(row["CategoryId"]),
                    CategoryName = row["CategoryName"].ToString(), // Map CategoryName
                    Discount = row["Discount"] != DBNull.Value ? Convert.ToInt32(row["Discount"]) : 0
                };

                // 2. Get Reviews
                string reviewQuery = @"
                    SELECT r.*, u.FullName 
                    FROM Reviews r 
                    JOIN Users u ON r.UserId = u.Id 
                    WHERE r.ProductId = @ProductId 
                    ORDER BY r.CreatedDate DESC";
                SqlParameter[] reviewParams = new SqlParameter[] { new SqlParameter("@ProductId", id) };
                DataTable dtReviews = _dbHelper.ExecuteQuery(reviewQuery, reviewParams);
                
                foreach (DataRow rRow in dtReviews.Rows)
                {
                    product.Reviews.Add(new Review
                    {
                        Id = Convert.ToInt32(rRow["Id"]),
                        UserId = Convert.ToInt32(rRow["UserId"]),
                        ProductId = Convert.ToInt32(rRow["ProductId"]),
                        Rating = Convert.ToInt32(rRow["Rating"]),
                        Content = rRow["Content"].ToString(),
                        CreatedDate = Convert.ToDateTime(rRow["CreatedDate"]),
                        UserName = rRow["FullName"].ToString()
                    });
                }
            }

            if (product == null)
            {
                return NotFound();
            }

            // 3. Check if User Can Review
            bool canReview = false;
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                string checkQuery = @"
                    SELECT COUNT(*) 
                    FROM OrderDetails d 
                    JOIN Orders o ON d.OrderId = o.Id 
                    WHERE o.UserId = @UserId 
                    AND d.ProductId = @ProductId 
                    AND o.Status = N'Đã giao'";
                
                SqlParameter[] checkParams = new SqlParameter[] 
                { 
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@ProductId", id)
                };
                
                int count = (int)_dbHelper.ExecuteScalar(checkQuery, checkParams);
                if (count > 0) canReview = true;
            }
            ViewBag.CanReview = canReview;

            return View(product);
        }

        [HttpPost]
        public IActionResult AddReview(int productId, int rating, string content)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            // Verify purchase again (security)
            string checkQuery = @"
                SELECT COUNT(*) 
                FROM OrderDetails d 
                JOIN Orders o ON d.OrderId = o.Id 
                WHERE o.UserId = @UserId 
                AND d.ProductId = @ProductId 
                AND o.Status = N'Đã giao'";
            
            SqlParameter[] checkParams = new SqlParameter[] 
            { 
                new SqlParameter("@UserId", userId),
                new SqlParameter("@ProductId", productId)
            };
            
            int count = (int)_dbHelper.ExecuteScalar(checkQuery, checkParams);
            if (count > 0)
            {
                string insertQuery = @"
                    INSERT INTO Reviews (UserId, ProductId, Rating, Content) 
                    VALUES (@UserId, @ProductId, @Rating, @Content)";
                
                SqlParameter[] insertParams = new SqlParameter[]
                {
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@ProductId", productId),
                    new SqlParameter("@Rating", rating),
                    new SqlParameter("@Content", content ?? "")
                };
                
                _dbHelper.ExecuteNonQuery(insertQuery, insertParams);
            }

            return RedirectToAction("Details", new { id = productId });
        }
        public IActionResult Category(int id, string sortOrder, decimal? minPrice, decimal? maxPrice)
        {
            if (id <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            List<Product> products = new List<Product>();
            string categoryName = "";

            // Get Category Name
            string catQuery = "SELECT Name FROM Categories WHERE Id = @Id";
            SqlParameter[] catParams = new SqlParameter[] { new SqlParameter("@Id", id) };
            object result = _dbHelper.ExecuteScalar(catQuery, catParams);
            if (result != null)
            {
                categoryName = result.ToString();
            }
            else
            {
                return NotFound();
            }

            ViewBag.CategoryName = categoryName;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            // Build Query
            string query = "SELECT * FROM Products WHERE CategoryId = @CategoryId";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@CategoryId", id));

            // Filter by Price
            if (minPrice.HasValue)
            {
                query += " AND (Price * (100 - Discount) / 100) >= @MinPrice";
                parameters.Add(new SqlParameter("@MinPrice", minPrice.Value));
            }

            if (maxPrice.HasValue)
            {
                query += " AND (Price * (100 - Discount) / 100) <= @MaxPrice";
                parameters.Add(new SqlParameter("@MaxPrice", maxPrice.Value));
            }

            // Sort
            switch (sortOrder)
            {
                case "price_asc":
                    query += " ORDER BY (Price * (100 - Discount) / 100) ASC";
                    break;
                case "price_desc":
                    query += " ORDER BY (Price * (100 - Discount) / 100) DESC";
                    break;
                case "name_asc":
                    query += " ORDER BY Name ASC";
                    break;
                case "name_desc":
                    query += " ORDER BY Name DESC";
                    break;
                default:
                    query += " ORDER BY Id DESC"; // Default
                    break;
            }

            DataTable dt = _dbHelper.ExecuteQuery(query, parameters.ToArray());
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

        public IActionResult Search(string keyword)
        {
            List<Product> products = new List<Product>();
            if (string.IsNullOrEmpty(keyword))
            {
                return View(products);
            }

            ViewBag.Keyword = keyword;

            string query = "SELECT * FROM Products WHERE Name LIKE @Keyword";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Keyword", "%" + keyword + "%")
            };

            DataTable dt = _dbHelper.ExecuteQuery(query, parameters);
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
        [HttpGet]
        public IActionResult SearchSuggestions(string keyword)
        {
            List<object> suggestions = new List<object>();
            if (string.IsNullOrEmpty(keyword))
            {
                return Json(suggestions);
            }

            string query = "SELECT TOP 5 Id, Name, Image, Price FROM Products WHERE Name LIKE @Keyword";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Keyword", "%" + keyword + "%")
            };

            DataTable dt = _dbHelper.ExecuteQuery(query, parameters);
            foreach (DataRow row in dt.Rows)
            {
                suggestions.Add(new
                {
                    id = row["Id"],
                    name = row["Name"],
                    image = row["Image"],
                    price = Convert.ToDecimal(row["Price"]).ToString("N0")
                });
            }

            return Json(suggestions);
        }
    }
}
