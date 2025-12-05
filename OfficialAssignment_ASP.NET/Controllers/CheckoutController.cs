using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficialAssignment_ASP.NET.Models;
using OfficialAssignment_ASP.NET.Models.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.Json;

namespace OfficialAssignment_ASP.NET.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public CheckoutController(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        // Helper: Get Cart from Session
        private List<CartDetail> GetCart()
        {
            var sessionCart = HttpContext.Session.GetString("Cart");
            if (sessionCart != null)
            {
                return JsonSerializer.Deserialize<List<CartDetail>>(sessionCart);
            }
            return new List<CartDetail>();
        }

        // Helper: Clear Cart
        private void ClearCart()
        {
            HttpContext.Session.Remove("Cart");
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Check login
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = GetCart();
            if (cart.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            decimal total = cart.Sum(item => item.Quantity * item.Product.Price);
            ViewBag.Total = total;
            
            // Pre-fill user info if available (optional, can be expanded)
            ViewBag.FullName = HttpContext.Session.GetString("FullName");

            return View(cart);
        }

        [HttpPost]
        public IActionResult ProcessOrder(string receiverName, string receiverPhone, string receiverAddress, string paymentMethod)
        {
            // Check login
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = GetCart();
            if (cart.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            int userId = (int)HttpContext.Session.GetInt32("UserId");
            decimal totalAmount = cart.Sum(item => item.Quantity * item.Product.Price);
            
            // Determine Payment Status
            string paymentStatus = "Chưa thanh toán";
            // Note: QR payment will be updated to "Đã thanh toán" when user confirms on Success page (or manually by admin)
            // For this flow, we keep it as "Chưa thanh toán" initially for both.

            // 1. Insert into Orders
            string orderQuery = @"
                INSERT INTO Orders (UserId, OrderDate, TotalAmount, Status, ReceiverName, ReceiverPhone, ReceiverAddress, PaymentMethod, PaymentStatus) 
                OUTPUT INSERTED.Id 
                VALUES (@UserId, GETDATE(), @TotalAmount, N'Chờ xử lý', @ReceiverName, @ReceiverPhone, @ReceiverAddress, @PaymentMethod, @PaymentStatus)";

            SqlParameter[] orderParams = new SqlParameter[]
            {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@TotalAmount", totalAmount),
                new SqlParameter("@ReceiverName", receiverName),
                new SqlParameter("@ReceiverPhone", receiverPhone),
                new SqlParameter("@ReceiverAddress", receiverAddress),
                new SqlParameter("@PaymentMethod", paymentMethod ?? "COD"),
                new SqlParameter("@PaymentStatus", paymentStatus)
            };

            int orderId = (int)_dbHelper.ExecuteScalar(orderQuery, orderParams);

            // 2. Insert into OrderDetails
            foreach (var item in cart)
            {
                string detailQuery = @"
                    INSERT INTO OrderDetails (OrderId, ProductId, Quantity, Price) 
                    VALUES (@OrderId, @ProductId, @Quantity, @Price)";
                
                SqlParameter[] detailParams = new SqlParameter[]
                {
                    new SqlParameter("@OrderId", orderId),
                    new SqlParameter("@ProductId", item.ProductId),
                    new SqlParameter("@Quantity", item.Quantity),
                    new SqlParameter("@Price", item.Product.Price)
                };

                _dbHelper.ExecuteNonQuery(detailQuery, detailParams);
            }

            // 3. Clear Cart and Redirect
            ClearCart();
            return RedirectToAction("Success", new { id = orderId });
        }

        public IActionResult Success(int id)
        {
            // Get Order Info for QR Display
            string query = "SELECT * FROM Orders WHERE Id = @Id";
            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@Id", id) };
            DataTable dt = _dbHelper.ExecuteQuery(query, parameters);
            
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                Order order = new Order
                {
                    Id = Convert.ToInt32(row["Id"]),
                    TotalAmount = Convert.ToDecimal(row["TotalAmount"]),
                    PaymentMethod = row["PaymentMethod"].ToString(),
                    PaymentStatus = row["PaymentStatus"].ToString()
                };
                return View(order);
            }
            
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public IActionResult ConfirmPayment(int orderId)
        {
            // Update PaymentStatus to "Đã thanh toán"
            string query = "UPDATE Orders SET PaymentStatus = N'Đã thanh toán' WHERE Id = @Id";
            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@Id", orderId) };
            _dbHelper.ExecuteNonQuery(query, parameters);

            return RedirectToAction("Success", new { id = orderId });
        }
    }
}
