using System;
namespace LMS.Areas.Admin.Models
{
    public class SearchUser
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public int? RoleId { get; set; }
        public int? CourseId { get; set; }
        public string Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        // Search result fields
        public int UserId { get; set; }
        public string Role { get; set; }
        public string Course { get; set; }
        public DateTime? EnrollmentDate { get; set; }
        public string UserStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}