using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficialAssignment_ASP.NET.Models;
using System.Collections.Generic;
using System.Linq;

namespace AssignmentTest
{
    [TestClass]
    public class CartTests
    {
        [TestMethod]
        public void Cart_CalculateTotal_CorrectSum()
        {
            // Arrange
            var items = new List<OrderDetail>
            {
                new OrderDetail { ProductId = 1, Price = 10000, Quantity = 2 }, // 20k
                new OrderDetail { ProductId = 2, Price = 50000, Quantity = 1 }  // 50k
            };

            // Act
            decimal total = items.Sum(x => x.Price * x.Quantity);

            // Assert
            Assert.AreEqual(70000, total);
        }

        [TestMethod]
        public void Cart_AddToCart_NewItem_IncreasesCount()
        {
            // Arrange
            var cart = new List<OrderDetail>();
            var newItem = new OrderDetail { ProductId = 1, Quantity = 1 };

            // Act
            cart.Add(newItem);

            // Assert
            Assert.AreEqual(1, cart.Count);
        }

        [TestMethod]
        public void Cart_Remove_ExistingItem_DecreasesCount()
        {
            // Arrange (Chuẩn bị)
            var cart = new List<OrderDetail>();
            var itemToRemove = new OrderDetail { ProductId = 1, Quantity = 1 };

            // Giả lập giỏ hàng đang có sẵn 1 món này
            cart.Add(itemToRemove);

            // Act (Hành động: Xóa món đó đi)
            cart.Remove(itemToRemove);

            // Assert (Kiểm tra)
            // Ban đầu có 1, xóa 1 thì phải còn 0
            Assert.AreEqual(0, cart.Count);
        }
    }
}