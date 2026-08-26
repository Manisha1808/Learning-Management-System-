using System.ComponentModel.DataAnnotations;
namespace LMS.Models
{
    public class Login
    {
        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is Required" )]
        public string Password { get; set; }
        
    }
}