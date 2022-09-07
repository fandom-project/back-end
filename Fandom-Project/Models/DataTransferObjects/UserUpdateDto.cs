using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fandom_Project.Models.DataTransferObjects
{
    public class UserUpdateDto
    {
        [Required(ErrorMessage = "The field FullName is required")]
        [MaxLength(255, ErrorMessage = "The field FullName must have a maximum length of 255")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "The field Email is required")]
        [MaxLength(255, ErrorMessage = "The field Email must have a maximum length of 255")]
        public string Email { get; set; }       
        
        [Required(ErrorMessage = "The field CreatedDate is required")]
        public DateTime CreatedDate { get; set; }

        [Required(ErrorMessage = "The field ModifiedDate is required")]
        public DateTime ModifiedDate { get; set; }

        [MaxLength(45, ErrorMessage = "The field Slug must have a maximum length of 45")]
        public string? Slug { get; set; }

        [MaxLength(255, ErrorMessage = "The field ProfileAvatar must have a maximum length of 255")]
        public string? ProfileAvatar { get; set; }

        [MaxLength(255, ErrorMessage = "The field Bio must have a maximum length of 255")]
        public string? Bio { get; set; }
    }
}
