using System;

namespace OfficialAssignment_ASP.NET.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }

        // Navigation properties (for display)
        public string UserName { get; set; }
    }
}
