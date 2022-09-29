namespace Fandom_Project.Models.DataTransferObjects.CommunityModel
{
    public class CommunityDto
    {
        public int CommunityId { get; set; }        
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int MemberCount { get; set; }
        public string? Slug { get; set; }
        public string? CoverImage { get; set; }
    }
}
