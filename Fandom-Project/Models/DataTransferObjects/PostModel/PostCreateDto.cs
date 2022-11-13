using System.ComponentModel.DataAnnotations;

namespace Fandom_Project.Models.DataTransferObjects.PostModel
{
    public class PostCreateDto
    {
        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [Required]
        [StringLength(45)]
        public string Type { get; set; }

        [Required]
        [StringLength(255)]
        public string Author { get; set; }

        [Required]
        [StringLength(1000)]
        public string Text { get; set; }               
    }
}
