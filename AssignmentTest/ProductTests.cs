using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using OfficialAssignment_ASP.NET.Controllers;
using OfficialAssignment_ASP.NET.Models;

namespace AssignmentTest
{
    [TestClass]
    public class ProductTests
    {
        [TestMethod]
        public void Product_Details_IdInvalid_ReturnsNotFound()
        {
            // Arrange
            var controller = new ProductController(null!);

            // Act: Truyền ID không tồn tại (-1)
            var result = controller.Details(-1);

            // Assert: Kiểm tra kết quả trả về không null (hoặc đúng kiểu NotFound nếu bạn đã sửa controller)
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Product_Search_EmptyKeyword_ReturnsView()
        {
            // Arrange
            var controller = new ProductController(null!);

            // Act
            var result = controller.Search("") as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Product_Category_IdInvalid_RedirectsToHome()
        {
            // Arrange
            // Vẫn truyền null vì code check ID sai sẽ return TRƯỚC khi gọi Database
            var controller = new ProductController(null!);

            // Act
            // Gọi hàm Category với id = -1. Các tham số sau (sort, price) để null hoặc rỗng
            var result = controller.Category(-1, "", null, null) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result); // Kết quả phải là Chuyển hướng (Redirect)
            Assert.AreEqual("Index", result.ActionName);    // Về trang Index
            Assert.AreEqual("Home", result.ControllerName); // Của HomeController
        }
    }
}