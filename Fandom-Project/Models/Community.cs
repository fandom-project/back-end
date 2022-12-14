using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fandom_Project.Models
{
    [Table("community")]
    public class Community
    {
        [Key]
        public int CommunityId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int MemberCount { get; set; }
        public int PostCount { get; set; }
        public string Slug { get; set; }
        public string? CoverImage { get; set; }
        public string? Description { get; set; }
        public string? BannerImage { get; set; }
    }
}
