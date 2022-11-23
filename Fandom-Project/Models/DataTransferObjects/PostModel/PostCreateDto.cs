using System.ComponentModel.DataAnnotations;

namespace Fandom_Project.Models.DataTransferObjects.PostModel
{
    public class PostCreateDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [Required]
        [StringLength(45)]
        public string Type { get; set; }

        [Required]
        [StringLength(65535)]
        public string Text { get; set; }

        [StringLength(255)]
        public string? CoverImage { get; set; }

        public DateTime? EventDate { get; set; }
    }
}
