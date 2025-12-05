using System;
using System.Collections.Generic;

namespace OfficialAssignment_ASP.NET.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<CartDetail> Items { get; set; } = new List<CartDetail>();
    }

    public class CartDetail
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        
        // Thuộc tính bổ sung để hiển thị thông tin sản phẩm
        public Product Product { get; set; }
    }
}
