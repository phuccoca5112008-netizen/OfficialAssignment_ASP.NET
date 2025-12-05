using System;

namespace OfficialAssignment_ASP.NET.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public int Role { get; set; } // 0: Admin, 1: Employee, 2: Customer
        public DateTime CreatedDate { get; set; }
    }
}
