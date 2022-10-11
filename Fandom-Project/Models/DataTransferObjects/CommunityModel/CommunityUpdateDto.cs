using System.ComponentModel.DataAnnotations;

namespace Fandom_Project.Models.DataTransferObjects.CommunityModel
{
    public class CommunityUpdateDto
    {
        [Required]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(45)]
        public string Name { get; set; }

        [Required]
        [StringLength(45)]
        public string? Slug { get; set; }

        [StringLength(255)]
        public string? CoverImage { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }
    }
}
