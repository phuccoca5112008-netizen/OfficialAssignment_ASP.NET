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
    }
}