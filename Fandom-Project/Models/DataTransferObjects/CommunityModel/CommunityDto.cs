namespace Fandom_Project.Models.DataTransferObjects.CommunityModel
{
    public class CommunityDto
    {
        public int CommunityId { get; set; }        
        public string Name { get; set; }
        public string OwnerName { get; set; }
        public int OwnerId { get; set; }
        public string CategoryName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int MemberCount { get; set; }
        public int PostCount { get; set; }
        public string Slug { get; set; }
        public string CoverImage { get; set; }
        public string Description { get; set; }
        public string BannerImage { get; set; }
    }
}
