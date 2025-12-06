using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficialAssignment_ASP.NET.Models;
using OfficialAssignment_ASP.NET.Models.DAL;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic; // Thêm namespace này để dùng List<Order>

namespace OfficialAssignment_ASP.NET.Controllers
{
    public class AccountController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public AccountController(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("Username") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // --- ĐOẠN CODE MỚI CHÈN VÀO ĐỂ FIX LỖI TEST ---
            // Kiểm tra đầu vào rỗng để tránh gọi DatabaseHelper khi tham số null (gây lỗi NullReference trong Test)
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập tên đăng nhập và mật khẩu!";
                return View();
            }
            // ----------------------------------------------

            string query = "SELECT * FROM Users WHERE Username = @Username AND Password = @Password";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Username", username),
                new SqlParameter("@Password", password)
            };

            DataTable dt = _dbHelper.ExecuteQuery(query, parameters);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                HttpContext.Session.SetInt32("UserId", Convert.ToInt32(row["Id"]));
                HttpContext.Session.SetString("Username", row["Username"].ToString());
                HttpContext.Session.SetInt32("Role", Convert.ToInt32(row["Role"]));
                HttpContext.Session.SetString("FullName", row["FullName"].ToString());

                // Redirect based on Role
                int role = Convert.ToInt32(row["Role"]);
                if (role == 0) // Admin
                {
                    // return RedirectToAction("Index", "Admin"); // Chưa làm Admin
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng!";
                return View();
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (HttpContext.Session.GetString("Username") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user, string confirmPassword)
        {
            // Thêm kiểm tra null cho user để an toàn hơn cho Test
            if (user == null)
            {
                ViewBag.Error = "Dữ liệu không hợp lệ!";
                return View();
            }

            if (user.Password != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp!";
                return View(user);
            }

            // Check if username exists
            string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
            SqlParameter[] checkParams = new SqlParameter[] { new SqlParameter("@Username", user.Username) };
            int count = (int)_dbHelper.ExecuteScalar(checkQuery, checkParams);

            if (count > 0)
            {
                ViewBag.Error = "Tên đăng nhập đã tồn tại!";
                return View(user);
            }

            // Insert new user (Default Role = 2: Customer)
            string insertQuery = @"INSERT INTO Users (Username, Password, FullName, Email, Phone, Address, Role) 
                                   VALUES (@Username, @Password, @FullName, @Email, @Phone, @Address, 2)";

            SqlParameter[] insertParams = new SqlParameter[]
            {
                new SqlParameter("@Username", user.Username),
                new SqlParameter("@Password", user.Password),
                new SqlParameter("@FullName", user.FullName ?? ""),
                new SqlParameter("@Email", user.Email ?? ""),
                new SqlParameter("@Phone", user.Phone ?? ""),
                new SqlParameter("@Address", user.Address ?? "")
            };

            _dbHelper.ExecuteNonQuery(insertQuery, insertParams);

            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult History()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            List<Order> orders = new List<Order>();
            string query = "SELECT * FROM Orders WHERE UserId = @UserId ORDER BY OrderDate DESC";
            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@UserId", userId) };
            DataTable dt = _dbHelper.ExecuteQuery(query, parameters);

            foreach (DataRow row in dt.Rows)
            {
                orders.Add(new Order
                {
                    Id = Convert.ToInt32(row["Id"]),
                    UserId = Convert.ToInt32(row["UserId"]),
                    OrderDate = Convert.ToDateTime(row["OrderDate"]),
                    TotalAmount = Convert.ToDecimal(row["TotalAmount"]),
                    Status = row["Status"].ToString(),
                    ReceiverName = row["ReceiverName"].ToString()
                });
            }

            return View(orders);
        }

        public IActionResult OrderDetails(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            Order order = null;

            // Get Order Info and verify it belongs to the user
            string orderQuery = "SELECT * FROM Orders WHERE Id = @Id AND UserId = @UserId";
            SqlParameter[] orderParams = new SqlParameter[]
            {
                new SqlParameter("@Id", id),
                new SqlParameter("@UserId", userId)
            };
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
                    ReceiverAddress = row["ReceiverAddress"].ToString()
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
        [HttpGet]
        public IActionResult Profile()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");

            string query = "SELECT * FROM Users WHERE Id = @Id";
            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@Id", userId) };
            DataTable dt = _dbHelper.ExecuteQuery(query, parameters);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                User user = new User
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Username = row["Username"].ToString(),
                    FullName = row["FullName"].ToString(),
                    Email = row["Email"].ToString(),
                    Phone = row["Phone"].ToString(),
                    Address = row["Address"].ToString(),
                    Role = Convert.ToInt32(row["Role"])
                };
                return View(user);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Profile(User model)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");

            string query = @"UPDATE Users 
                             SET FullName = @FullName, Email = @Email, Phone = @Phone, Address = @Address 
                             WHERE Id = @Id";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@FullName", model.FullName ?? ""),
                new SqlParameter("@Email", model.Email ?? ""),
                new SqlParameter("@Phone", model.Phone ?? ""),
                new SqlParameter("@Address", model.Address ?? ""),
                new SqlParameter("@Id", userId)
            };

            _dbHelper.ExecuteNonQuery(query, parameters);

            // Update Session
            HttpContext.Session.SetString("FullName", model.FullName ?? "");

            TempData["Success"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("Profile");
        }

        [HttpPost]
        public IActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");

            if (newPassword != confirmPassword)
            {
                TempData["Error"] = "Mật khẩu xác nhận không khớp!";
                return RedirectToAction("Profile");
            }

            // Verify current password
            string checkQuery = "SELECT COUNT(*) FROM Users WHERE Id = @Id AND Password = @Password";
            SqlParameter[] checkParams = new SqlParameter[]
            {
                new SqlParameter("@Id", userId),
                new SqlParameter("@Password", currentPassword)
            };
            int count = (int)_dbHelper.ExecuteScalar(checkQuery, checkParams);

            if (count == 0)
            {
                TempData["Error"] = "Mật khẩu hiện tại không đúng!";
                return RedirectToAction("Profile");
            }

            // Update password
            string updateQuery = "UPDATE Users SET Password = @Password WHERE Id = @Id";
            SqlParameter[] updateParams = new SqlParameter[]
            {
                new SqlParameter("@Password", newPassword),
                new SqlParameter("@Id", userId)
            };
            _dbHelper.ExecuteNonQuery(updateQuery, updateParams);

            TempData["Success"] = "Đổi mật khẩu thành công!";
            return RedirectToAction("Profile");
        }
    }
}