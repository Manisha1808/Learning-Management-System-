using System;
using System.ComponentModel.DataAnnotations;
namespace LMS.Areas.Employee.Models
{
    public class ManageProfile
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        [RegularExpression(@"^[6-9][0-9]{9}$",ErrorMessage = "Enter a valid 10-digit phone number.")]
        public string PhoneNumber { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}