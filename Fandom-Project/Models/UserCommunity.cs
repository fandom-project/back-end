using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fandom_Project.Models
{
    [Table("usercommunity")]
    public class UserCommunity
    {
        [Key]
        public int UserId { get; set; }
        public int CommunityId { get; set; }
        public string Role { get; set; }
    }
}
