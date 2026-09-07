using System;
namespace LMS.Areas.Employee.Models
{
    public class Certificate
    {
        public int CertificateId { get; set; }

        public int UserId { get; set; }

        public int CourseId { get; set; }

        public int QuizResultId { get; set; }

        public string CertificateNumber { get; set; }

        public string EmployeeName { get; set; }

        public string CourseName { get; set; }

        public int Score { get; set; }

        public int TotalQuestions { get; set; }

        public int Percentage { get; set; }

        public bool IsPassed { get; set; }

        public DateTime IssueDate { get; set; }
    }
}