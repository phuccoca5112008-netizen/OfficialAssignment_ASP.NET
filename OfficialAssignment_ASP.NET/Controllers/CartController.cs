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
    public class CartController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public CartController(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        // Lấy giỏ hàng từ Session
        private List<CartDetail> GetCart()
        {
            var sessionCart = HttpContext.Session.GetString("Cart");
            if (sessionCart != null)
            {
                return JsonSerializer.Deserialize<List<CartDetail>>(sessionCart);
            }
            return new List<CartDetail>();
        }

        // Lưu giỏ hàng vào Session
        private void SaveCart(List<CartDetail> cart)
        {
            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cart));
        }

        public IActionResult Index()
        {
            var cart = GetCart();
            
            // Tính tổng tiền
            decimal total = cart.Sum(item => item.Quantity * item.Product.Price);
            ViewBag.Total = total;

            return View(cart);
        }

        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            // Kiểm tra đăng nhập
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = GetCart();
            var cartItem = cart.FirstOrDefault(c => c.ProductId == productId);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                // Lấy thông tin sản phẩm từ DB
                string query = "SELECT * FROM Products WHERE Id = @Id";
                SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@Id", productId) };
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
                        Discount = row["Discount"] != DBNull.Value ? Convert.ToInt32(row["Discount"]) : 0
                    };

                    // Apply Discount
                    if (product.Discount > 0)
                    {
                        product.Price = product.Price * (100 - product.Discount) / 100;
                    }

                    cart.Add(new CartDetail
                    {
                        ProductId = productId,
                        Quantity = quantity,
                        Product = product
                    });
                }
            }

            SaveCart(cart);
            return RedirectToAction("Index");
        }

        public IActionResult Remove(int id)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.ProductId == id);
            if (item != null)
            {
                cart.Remove(item);
                SaveCart(cart);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Update(int id, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.ProductId == id);
            if (item != null)
            {
                if (quantity > 0)
                {
                    item.Quantity = quantity;
                }
                else
                {
                    cart.Remove(item);
                }
                SaveCart(cart);
            }
            return RedirectToAction("Index");
        }
        
        public IActionResult Clear()
        {
            HttpContext.Session.Remove("Cart");
            return RedirectToAction("Index");
        }
    }
}
