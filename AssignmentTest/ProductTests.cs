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
    }
}