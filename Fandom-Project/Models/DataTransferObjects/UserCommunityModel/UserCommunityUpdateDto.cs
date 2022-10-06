using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fandom_Project.Models.DataTransferObjects.UserCommunityModel
{    
    public class UserCommunityUpdateDto
    {        
        public int CommunityId { get; set; }
        public string Role { get; set; }
    }
}
