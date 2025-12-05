namespace OfficialAssignment_ASP.NET.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public int Discount { get; set; } // Phần trăm giảm giá (0-100)
        
        // Thuộc tính bổ sung để hiển thị tên danh mục (nếu cần)
        public string CategoryName { get; set; }
        
        public List<Review> Reviews { get; set; } = new List<Review>();
    }
}
