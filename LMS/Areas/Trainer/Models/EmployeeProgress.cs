using System;
namespace LMS.Areas.Trainer.Models
{
    public class EmployeeProgress
    { // Filter fields
        public string SearchText { get; set; }
        public string StatusFilter { get; set; }
        public DateTime? EnrollmentFrom { get; set; }
        public DateTime? EnrollmentTo { get; set; }

        // Grid fields
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string CourseName { get; set; }
        public string Status { get; set; }
        public DateTime? EnrollmentDate { get; set; }
    }
}