using System.ComponentModel.DataAnnotations;

namespace Fandom_Project.Models.DataTransferObjects.UserCommunityModel
{
    public class UserCommunityDeleteDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int CommunityId { get; set; }        
    }
}
