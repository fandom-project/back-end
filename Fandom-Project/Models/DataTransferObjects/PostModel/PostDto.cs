namespace Fandom_Project.Models.DataTransferObjects.PostModel
{
    public class PostDto
    {
        public int PostId { get; set; }
        public int UserId { get; set; }
        //public int CommunityId { get; set; }
        public string AuthorName { get; set; }
        //public string CommunityName { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }       
        public string Text { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime? EventDate { get; set; }
        public string? CoverImage { get; set; }
    }
}
