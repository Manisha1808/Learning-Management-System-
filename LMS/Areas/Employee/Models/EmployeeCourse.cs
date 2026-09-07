using System;

namespace LMS.Areas.Employee.Models
{
    public class EmployeeCourse
    {
        public int UserCourseId { get; set; }

        public int UserId { get; set; }

        public int CourseId { get; set; }

        public string CourseName { get; set; }

        public string Description { get; set; }

        public DateTime EnrollmentDate { get; set; }

        public string Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}