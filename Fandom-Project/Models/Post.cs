﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fandom_Project.Models
{
    [Table("post")]
    public class Post
    {
        [Key]
        public int PostId { get; set; }

        public string Title { get; set; }
        
        public string Type { get; set; }

        public string Author { get; set; }

        public string Text { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public int CommunityId { get; set; }
    }
}
