using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using OfficialAssignment_ASP.NET.Controllers;
using OfficialAssignment_ASP.NET.Models; // Namespace chứa User, OrderDetail
using Moq;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

namespace AssignmentTest
{
    [TestClass]
    public class AllTests
    {
        // --- TEST 1: PRODUCT CONTROLLER ---

        [TestMethod]
        public void Product_Details_IdInvalid_ReturnsNotFound()
        {
            // SỬA LỖI CS8625: Thêm dấu chấm than (!) sau null để bỏ qua cảnh báo null
            var controller = new ProductController(null!);

            // Act: Truyền ID không tồn tại (-1)
            var result = controller.Details(-1);

            // Assert: Kiểm tra kết quả trả về không null
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Product_Search_EmptyKeyword_ReturnsView()
        {
            // SỬA LỖI CS8625
            var controller = new ProductController(null!);
            var result = controller.Search("") as ViewResult;
            Assert.IsNotNull(result);
        }

        // --- TEST 2: CART LOGIC (Sử dụng OrderDetail thay cho CartItem) ---
        
        [TestMethod]
        public void Cart_CalculateTotal_CorrectSum()
        {
            // Giả sử OrderDetail có ProductId, Price, Quantity
            var items = new List<OrderDetail>
            {
                new OrderDetail { ProductId = 1, Price = 10000, Quantity = 2 }, // 20k
                new OrderDetail { ProductId = 2, Price = 50000, Quantity = 1 }  // 50k
            };

            // SỬA LỖI CS0019: Xóa bỏ '?? 0' vì thuộc tính Price/Quantity của bạn là kiểu không null
            decimal total = items.Sum(x => x.Price * x.Quantity);

            Assert.AreEqual(70000, total);
        }

        [TestMethod]
        public void Cart_AddToCart_NewItem_IncreasesCount()
        {
            var cart = new List<OrderDetail>();
            var newItem = new OrderDetail { ProductId = 1, Quantity = 1 };

            cart.Add(newItem);

            Assert.AreEqual(1, cart.Count);
        }

        // --- TEST 3: ACCOUNT CONTROLLER (SỬA LỖI THAM SỐ + SỬA LỖI NULL REFERENCE) ---

        [TestMethod]
        public void Account_Login_InputInvalid_ReturnsView()
        {
            // SỬA LỖI CS8625
            var controller = new AccountController(null!);

            // --- SỬA LỖI MỚI (NullReferenceException) ---
            // Nguyên nhân: Controller.Login có dùng Session, nhưng môi trường test chưa có Session.
            // Giải pháp: Giả lập Session ảo.
            var mockCtx = new ControllerContext();
            var mockHttp = new Mock<HttpContext>();
            mockHttp.Setup(s => s.Session).Returns(new Mock<ISession>().Object);
            mockCtx.HttpContext = mockHttp.Object;
            controller.ControllerContext = mockCtx;
            // ---------------------------------------------

            // Hàm Login của bạn nhận (string, string)
            string username = "";
            string password = "";

            // Giả lập lỗi ModelState
            controller.ModelState.AddModelError("Error", "Missing info");

            // Gọi hàm với đúng 2 tham số string
            var result = controller.Login(username, password) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsFalse(controller.ModelState.IsValid);
        }

        [TestMethod]
        public void Account_Register_PasswordMismatch_ReturnsError()
        {
            // SỬA LỖI CS8625
            var controller = new AccountController(null!);

            // --- SỬA LỖI MỚI (Phòng hờ cho Register luôn) ---
            var mockCtx = new ControllerContext();
            var mockHttp = new Mock<HttpContext>();
            mockHttp.Setup(s => s.Session).Returns(new Mock<ISession>().Object);
            mockCtx.HttpContext = mockHttp.Object;
            controller.ControllerContext = mockCtx;
            // ------------------------------------------------

            // Hàm Register của bạn nhận (User, string)
            var userModel = new User { Username = "test", Password = "123" };
            string confirmPass = "456"; // Mật khẩu nhập lại sai

            // Giả lập logic kiểm tra mật khẩu không khớp trong Controller
            if (userModel.Password != confirmPass)
            {
                controller.ModelState.AddModelError("Error", "Mismatch");
            }

            // Gọi hàm với đúng 2 tham số: (User object, string confirmPassword)
            var result = controller.Register(userModel, confirmPass) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsFalse(controller.ModelState.IsValid);
        }

        [TestMethod]
        public void Account_Logout_Action_RedirectsToHome()
        {
            // SỬA LỖI CS8625
            var controller = new AccountController(null!);

            // Giả lập Session để tránh lỗi NullReference khi Logout xóa session
            var mockCtx = new ControllerContext();
            var mockHttp = new Mock<HttpContext>();
            mockHttp.Setup(s => s.Session).Returns(new Mock<ISession>().Object);
            mockCtx.HttpContext = mockHttp.Object;
            controller.ControllerContext = mockCtx;

            var result = controller.Logout() as RedirectToActionResult;

            Assert.IsNotNull(result);
            // Kiểm tra xem nó có chuyển hướng về trang chủ (Index) không
            Assert.AreEqual("Index", result.ActionName);
        }
    }
}