using System;
using System.Collections.Generic;

namespace OfficialAssignment_ASP.NET.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverPhone { get; set; }
        public string ReceiverAddress { get; set; }
        public string PaymentMethod { get; set; } // COD, QR
        public string PaymentStatus { get; set; } // Chưa thanh toán, Đã thanh toán
        
        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
