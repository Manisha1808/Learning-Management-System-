using System;

namespace LMS.Areas.Admin.Models
{
    public class UserList
    { 
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Course { get; set; }
        public DateTime EnrollementDate { get; set; }
        public string status { get; set; } 
    }
}