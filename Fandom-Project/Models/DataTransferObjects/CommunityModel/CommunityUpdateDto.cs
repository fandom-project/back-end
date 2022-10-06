namespace Fandom_Project.Models.DataTransferObjects.CommunityModel
{
    public class CommunityUpdateDto
    {        
        public int CategoryId { get; set; }
        public string Name { get; set; }      
        public string? Slug { get; set; }
        public string? CoverImage { get; set; }
        public string? Description { get; set; }
    }
}
