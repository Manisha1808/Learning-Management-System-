using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.Areas.Employee.Models
{
    public class ManageProfile
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string CreatedBy { get; set; }
    }
}