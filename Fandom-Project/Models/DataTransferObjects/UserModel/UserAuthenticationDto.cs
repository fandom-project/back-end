using System.ComponentModel.DataAnnotations;

namespace Fandom_Project.Models.DataTransferObjects.UserModel
{
    public class UserAuthenticationDto
    {
        [Required(ErrorMessage = "The field Email is required")]
        [MaxLength(255, ErrorMessage = "The field Email must have a maximum length of 255")]
        [EmailAddress(ErrorMessage = "Email does not have a valid format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The field Password is required")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "The field Password must have between 6 and 20 characters")]
        public string Password { get; set; }
    }
}
