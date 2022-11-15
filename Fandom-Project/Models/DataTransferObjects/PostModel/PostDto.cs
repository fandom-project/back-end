namespace Fandom_Project.Models.DataTransferObjects.PostModel
{
    public class PostDto
    {
        public int UserId { get; set; }
        public string AuthorName { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }       
        public string Text { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }        
    }
}
