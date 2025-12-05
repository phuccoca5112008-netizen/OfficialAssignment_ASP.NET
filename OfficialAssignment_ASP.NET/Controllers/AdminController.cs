using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficialAssignment_ASP.NET.Models;
using OfficialAssignment_ASP.NET.Models.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace OfficialAssignment_ASP.NET.Controllers
{
    public class AdminController : Controller
    {
        private readonly DatabaseHelper _dbHelper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(DatabaseHelper dbHelper, IWebHostEnvironment webHostEnvironment)
        {
            _dbHelper = dbHelper;
            _webHostEnvironment = webHostEnvironment;
        }

        // ... Existing IsAdmin, Index, Orders, OrderDetails, UpdateStatus ...

        // PRODUCT MANAGEMENT
        public IActionResult Products()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            List<Product> products = new List<Product>();
            // Get Products with Category Name
            string query = @"
                SELECT p.*, c.Name as CategoryName 
                FROM Products p 
                LEFT JOIN Categories c ON p.CategoryId = c.Id 
                ORDER BY p.Id DESC";
            
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

                    CategoryName = row["CategoryName"].ToString(),
                    Discount = row["Discount"] != DBNull.Value ? Convert.ToInt32(row["Discount"]) : 0
                });
            }

            return View(products);
        }

        [HttpGet]
        public IActionResult CreateProduct()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            
            // Get Categories for Dropdown
            ViewBag.Categories = _dbHelper.ExecuteQuery("SELECT * FROM Categories");
            return View();
        }

        [HttpPost]
        public IActionResult CreateProduct(Product model, IFormFile imageFile)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            // Handle Image Upload
            if (imageFile != null && imageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(fileStream);
                }
                model.Image = uniqueFileName;
            }
            else
            {
                model.Image = "no-image.png"; // Default image
            }

            string query = @"
                INSERT INTO Products (Name, Price, Image, Color, Size, Description, CategoryId, Discount) 
                VALUES (@Name, @Price, @Image, @Color, @Size, @Description, @CategoryId, @Discount)";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Name", model.Name),
                new SqlParameter("@Price", model.Price),
                new SqlParameter("@Image", model.Image),
                new SqlParameter("@Color", model.Color ?? ""),
                new SqlParameter("@Size", model.Size ?? ""),
                new SqlParameter("@Description", model.Description ?? ""),

                new SqlParameter("@CategoryId", model.CategoryId),
                new SqlParameter("@Discount", model.Discount)
            };

            _dbHelper.ExecuteNonQuery(query, parameters);

            return RedirectToAction("Products");
        }

        [HttpGet]
        public IActionResult EditProduct(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            string query = "SELECT * FROM Products WHERE Id = @Id";
            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@Id", id) };
            DataTable dt = _dbHelper.ExecuteQuery(query, parameters);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                Product product = new Product
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
                };

                ViewBag.Categories = _dbHelper.ExecuteQuery("SELECT * FROM Categories");
                return View(product);
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult EditProduct(Product model, IFormFile imageFile)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            // Handle Image Upload if new file is selected
            if (imageFile != null && imageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(fileStream);
                }
                model.Image = uniqueFileName;
                
                // Update with new image
                string query = @"
                    UPDATE Products 
                    SET Name = @Name, Price = @Price, Image = @Image, Color = @Color, Size = @Size, Description = @Description, CategoryId = @CategoryId, Discount = @Discount 
                    WHERE Id = @Id";
                
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@Name", model.Name),
                    new SqlParameter("@Price", model.Price),
                    new SqlParameter("@Image", model.Image),
                    new SqlParameter("@Color", model.Color ?? ""),
                    new SqlParameter("@Size", model.Size ?? ""),
                    new SqlParameter("@Description", model.Description ?? ""),
                    new SqlParameter("@CategoryId", model.CategoryId),
                    new SqlParameter("@Discount", model.Discount),
                    new SqlParameter("@Id", model.Id)
                };
                _dbHelper.ExecuteNonQuery(query, parameters);
            }
            else
            {
                // Update without changing image
                string query = @"
                    UPDATE Products 
                    SET Name = @Name, Price = @Price, Color = @Color, Size = @Size, Description = @Description, CategoryId = @CategoryId, Discount = @Discount 
                    WHERE Id = @Id";
                
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@Name", model.Name),
                    new SqlParameter("@Price", model.Price),
                    new SqlParameter("@Color", model.Color ?? ""),
                    new SqlParameter("@Size", model.Size ?? ""),
                    new SqlParameter("@Description", model.Description ?? ""),
                    new SqlParameter("@CategoryId", model.CategoryId),
                    new SqlParameter("@Discount", model.Discount),
                    new SqlParameter("@Id", model.Id)
                };
                _dbHelper.ExecuteNonQuery(query, parameters);
            }

            return RedirectToAction("Products");
        }

        [HttpPost]
        public IActionResult DeleteProduct(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            // Optional: Delete image file
            // Optional: Check if product is in any order (Foreign Key constraint might fail)
            // For simplicity, we assume we can delete or we catch error
            
            try 
            {
                string query = "DELETE FROM Products WHERE Id = @Id";
                SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@Id", id) };
                _dbHelper.ExecuteNonQuery(query, parameters);
            }
            catch (Exception ex)
            {
                // Handle FK constraint error (product in orders)
                // For now, just redirect
            }

            return RedirectToAction("Products");
        }
        // Middleware-like check for Admin/Staff role
        private bool IsAdmin()
        {
            int? role = HttpContext.Session.GetInt32("Role");
            return role != null && (role == 0 || role == 1); // 0: Admin, 1: Staff
        }

        // Check for Super Admin (Role 0) only
        private bool IsSuperAdmin()
        {
            int? role = HttpContext.Session.GetInt32("Role");
            return role != null && role == 0;
        }

        public IActionResult Index()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            // Statistics - Only count "Đã giao"
            ViewBag.TotalOrders = _dbHelper.ExecuteScalar("SELECT COUNT(*) FROM Orders WHERE Status = N'Đã giao'");
            ViewBag.TotalRevenue = _dbHelper.ExecuteScalar("SELECT ISNULL(SUM(TotalAmount), 0) FROM Orders WHERE Status = N'Đã giao'");
            ViewBag.TotalProducts = _dbHelper.ExecuteScalar("SELECT COUNT(*) FROM Products");
            ViewBag.TotalUsers = _dbHelper.ExecuteScalar("SELECT COUNT(*) FROM Users");

            return View();
        }

        // ... Orders, OrderDetails, UpdateStatus ...

        // USER MANAGEMENT (Super Admin Only)
        public IActionResult Users(string keyword = "")
        {
            if (!IsSuperAdmin()) return RedirectToAction("Index"); // Redirect to Dashboard if not Super Admin

            string query = "SELECT * FROM Users";
            if (!string.IsNullOrEmpty(keyword))
            {
                query += " WHERE Username LIKE @Keyword OR FullName LIKE @Keyword OR Email LIKE @Keyword";
            }
            
            SqlParameter[] parameters = new SqlParameter[] 
            { 
                new SqlParameter("@Keyword", "%" + keyword + "%") 
            };
            
            DataTable dt = _dbHelper.ExecuteQuery(query, parameters);
            List<User> users = new List<User>();

            foreach (DataRow row in dt.Rows)
            {
                users.Add(new User
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Username = row["Username"].ToString(),
                    FullName = row["FullName"].ToString(),
                    Email = row["Email"].ToString(),
                    Phone = row["Phone"].ToString(),
                    Role = Convert.ToInt32(row["Role"])
                });
            }

            ViewBag.Keyword = keyword;
            return View(users);
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            if (!IsSuperAdmin()) return RedirectToAction("Index");
            return View();
        }

        [HttpPost]
        public IActionResult CreateUser(User model)
        {
            if (!IsSuperAdmin()) return RedirectToAction("Index");

            // Check if username exists
            string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
            SqlParameter[] checkParams = new SqlParameter[] { new SqlParameter("@Username", model.Username) };
            int count = (int)_dbHelper.ExecuteScalar(checkQuery, checkParams);

            if (count > 0)
            {
                ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại");
                return View(model);
            }

            string query = @"
                INSERT INTO Users (Username, Password, FullName, Email, Phone, Role) 
                VALUES (@Username, @Password, @FullName, @Email, @Phone, @Role)";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Username", model.Username),
                new SqlParameter("@Password", model.Password), // Note: Should hash password in real app
                new SqlParameter("@FullName", model.FullName),
                new SqlParameter("@Email", model.Email),
                new SqlParameter("@Phone", model.Phone ?? ""),
                new SqlParameter("@Role", model.Role)
            };

            _dbHelper.ExecuteNonQuery(query, parameters);

            return RedirectToAction("Users");
        }

        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            if (!IsSuperAdmin()) return RedirectToAction("Index");

            // Prevent deleting yourself
            int? currentUserId = HttpContext.Session.GetInt32("UserId");
            if (currentUserId == id)
            {
                return RedirectToAction("Users"); // Or show error
            }

            try
            {
                string query = "DELETE FROM Users WHERE Id = @Id";
                SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@Id", id) };
                _dbHelper.ExecuteNonQuery(query, parameters);
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                if (ex.Number == 547) // FK Constraint Violation
                {
                    TempData["Error"] = "Không thể xóa người dùng này vì họ đã có đơn hàng hoặc dữ liệu liên quan.";
                }
                else
                {
                    TempData["Error"] = "Lỗi khi xóa người dùng: " + ex.Message;
                }
            }

            return RedirectToAction("Users");
        }

        public IActionResult Orders()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            List<Order> orders = new List<Order>();
            // Hide "Đã hủy" orders
            string query = "SELECT * FROM Orders WHERE Status != N'Đã hủy' ORDER BY OrderDate DESC";
            DataTable dt = _dbHelper.ExecuteQuery(query);

            foreach (DataRow row in dt.Rows)
            {
                orders.Add(new Order
                {
                    Id = Convert.ToInt32(row["Id"]),
                    UserId = Convert.ToInt32(row["UserId"]),
                    OrderDate = Convert.ToDateTime(row["OrderDate"]),
                    TotalAmount = Convert.ToDecimal(row["TotalAmount"]),
                    Status = row["Status"].ToString(),
                    ReceiverName = row["ReceiverName"].ToString(),
                    PaymentMethod = row["PaymentMethod"] != DBNull.Value ? row["PaymentMethod"].ToString() : "COD",
                    PaymentStatus = row["PaymentStatus"] != DBNull.Value ? row["PaymentStatus"].ToString() : "Chưa thanh toán"
                });
            }

            return View(orders);
        }

        public IActionResult OrderDetails(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            Order order = null;
            
            // Get Order Info
            string orderQuery = "SELECT * FROM Orders WHERE Id = @Id";
            SqlParameter[] orderParams = new SqlParameter[] { new SqlParameter("@Id", id) };
            DataTable dtOrder = _dbHelper.ExecuteQuery(orderQuery, orderParams);
            
            if (dtOrder.Rows.Count > 0)
            {
                DataRow row = dtOrder.Rows[0];
                order = new Order
                {
                    Id = Convert.ToInt32(row["Id"]),
                    UserId = Convert.ToInt32(row["UserId"]),
                    OrderDate = Convert.ToDateTime(row["OrderDate"]),
                    TotalAmount = Convert.ToDecimal(row["TotalAmount"]),
                    Status = row["Status"].ToString(),
                    ReceiverName = row["ReceiverName"].ToString(),
                    ReceiverPhone = row["ReceiverPhone"].ToString(),
                    ReceiverAddress = row["ReceiverAddress"].ToString(),
                    PaymentMethod = row["PaymentMethod"] != DBNull.Value ? row["PaymentMethod"].ToString() : "COD",
                    PaymentStatus = row["PaymentStatus"] != DBNull.Value ? row["PaymentStatus"].ToString() : "Chưa thanh toán"
                };

                // Get Order Details
                string detailQuery = @"
                    SELECT d.*, p.Name as ProductName, p.Image 
                    FROM OrderDetails d 
                    JOIN Products p ON d.ProductId = p.Id 
                    WHERE d.OrderId = @OrderId";
                SqlParameter[] detailParams = new SqlParameter[] { new SqlParameter("@OrderId", id) };
                DataTable dtDetail = _dbHelper.ExecuteQuery(detailQuery, detailParams);

                foreach (DataRow dRow in dtDetail.Rows)
                {
                    order.OrderDetails.Add(new OrderDetail
                    {
                        ProductId = Convert.ToInt32(dRow["ProductId"]),
                        Quantity = Convert.ToInt32(dRow["Quantity"]),
                        Price = Convert.ToDecimal(dRow["Price"]),
                        Product = new Product 
                        { 
                            Name = dRow["ProductName"].ToString(),
                            Image = dRow["Image"].ToString()
                        }
                    });
                }
            }

            if (order == null) return NotFound();

            return View(order);
        }

        [HttpPost]
        public IActionResult UpdateStatus(int orderId, string status)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            string query = "UPDATE Orders SET Status = @Status WHERE Id = @Id";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Status", status),
                new SqlParameter("@Id", orderId)
            };

            _dbHelper.ExecuteNonQuery(query, parameters);

            return RedirectToAction("Orders");
        }

        [HttpGet]
        public IActionResult GetChartData()
        {
            if (!IsAdmin()) return Unauthorized();

            // 1. Revenue Last 7 Days
            var revenueData = new List<decimal>();
            var labels = new List<string>();
            
            for (int i = 6; i >= 0; i--)
            {
                DateTime date = DateTime.Now.Date.AddDays(-i);
                labels.Add(date.ToString("dd/MM"));
                
                string query = "SELECT SUM(TotalAmount) FROM Orders WHERE CAST(OrderDate AS DATE) = @Date AND Status != N'Đã hủy'";
                SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@Date", date) };
                object result = _dbHelper.ExecuteScalar(query, parameters);
                
                revenueData.Add(result != DBNull.Value ? Convert.ToDecimal(result) : 0);
            }

            // 2. Order Status Distribution
            var statusData = new List<int>();
            string[] statuses = { "Chờ xử lý", "Đang giao", "Đã giao", "Đã hủy" };
            
            foreach (var status in statuses)
            {
                string query = "SELECT COUNT(*) FROM Orders WHERE Status = @Status";
                SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@Status", status) };
                statusData.Add((int)_dbHelper.ExecuteScalar(query, parameters));
            }

            return Json(new { labels, revenueData, statusData });
        }
    }
}
