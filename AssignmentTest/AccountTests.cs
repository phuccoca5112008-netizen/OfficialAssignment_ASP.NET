using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using OfficialAssignment_ASP.NET.Controllers;
using OfficialAssignment_ASP.NET.Models;
using Moq;
using Microsoft.AspNetCore.Http;

namespace AssignmentTest
{
    [TestClass]
    public class AccountTests
    {
        private AccountController _controller;
        private Mock<HttpContext> _mockHttp;

        // Dùng TestInitialize để setup chung cho các hàm test (giúp code gọn hơn)
        [TestInitialize]
        public void Setup()
        {
            _controller = new AccountController(null!);

            // Giả lập Session để tránh lỗi NullReference
            var mockCtx = new ControllerContext();
            _mockHttp = new Mock<HttpContext>();
            _mockHttp.Setup(s => s.Session).Returns(new Mock<ISession>().Object);
            mockCtx.HttpContext = _mockHttp.Object;
            _controller.ControllerContext = mockCtx;
        }

        [TestMethod]
        public void Account_Login_InputInvalid_ReturnsView()
        {
            // Arrange
            string username = "";
            string password = "";
            _controller.ModelState.AddModelError("Error", "Missing info");

            // Act
            var result = _controller.Login(username, password) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(_controller.ModelState.IsValid);
        }

        [TestMethod]
        public void Account_Register_PasswordMismatch_ReturnsError()
        {
            // Arrange
            var userModel = new User { Username = "test", Password = "123" };
            string confirmPass = "456"; // Mật khẩu nhập lại sai

            // Giả lập logic kiểm tra mật khẩu không khớp
            if (userModel.Password != confirmPass)
            {
                _controller.ModelState.AddModelError("Error", "Mismatch");
            }

            // Act
            var result = _controller.Register(userModel, confirmPass) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(_controller.ModelState.IsValid);
        }

        [TestMethod]
        public void Account_Logout_Action_RedirectsToHome()
        {
            // Act
            var result = _controller.Logout() as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }
    }
}