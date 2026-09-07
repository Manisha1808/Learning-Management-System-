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
        public DateTime? EnrollmentDate { get; set; }
        public string UserStatus { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
};
