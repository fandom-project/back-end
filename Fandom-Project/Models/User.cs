using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fandom_Project.Models
{
    [Table("user")]
    public class User
    {
        [Key]
        public int UserId { get; set; }        
        public string FullName { get; set; }        
        public string Email { get; set; }        
        public string Password { get; set; }        
        public DateTime CreatedDate { get; set; }        
        public DateTime ModifiedDate { get; set; }       
        public string Slug { get; set; }        
        public string? ProfileAvatar { get; set; }                
        public string? Bio { get; set; }
    }
}
