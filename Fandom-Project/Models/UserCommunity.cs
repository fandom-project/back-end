using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fandom_Project.Models
{
    [Table("usercommunity")]    
    public class UserCommunity
    {
        [Key]
        public int UserCommunityId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int CommunityId { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
