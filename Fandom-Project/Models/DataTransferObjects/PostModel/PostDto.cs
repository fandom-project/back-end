namespace Fandom_Project.Models.DataTransferObjects.PostModel
{
    public class PostDto
    {       
        public string Title { get; set; }

        public string Type { get; set; }

        public string Author { get; set; }

        public string Text { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }        
    }
}
