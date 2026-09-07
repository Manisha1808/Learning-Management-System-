using System;

namespace LMS.Areas.Admin.Models
{
    public class SearchUser
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public int? RoleId { get; set; }
    }
}