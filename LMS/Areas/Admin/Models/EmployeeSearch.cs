using System;

namespace LMS.Areas.Admin.Models
{
    public class EmployeeSearch
    {
        public string EmployeeName { get; set; }

        public string Email { get; set; }

        public int? CourseId { get; set; }

        public string Status { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }
    }
}