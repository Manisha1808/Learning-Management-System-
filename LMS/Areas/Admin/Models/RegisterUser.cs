using System.ComponentModel.DataAnnotations;

namespace LMS.Areas.Admin.Models
{
    public class RegisterUser
    {
        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(20, MinimumLength = 8,ErrorMessage = "Password must be between 8 and 20 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]+$",ErrorMessage = "Password must contain uppercase, lowercase, number and special character.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please select a role.")]
        public string Role { get; set; }
        public int? CourseId { get; set; }
    }
}