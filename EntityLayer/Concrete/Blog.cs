using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Blog
    {
        [Key]
        public int Id {  get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public string Thumbnail { get; set; }
        public string Slug { get; set; }
        public int ReadingTime { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public int ViewCount { get; set; } = 0; 
        public DateTime? PublishDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool Status { get; set; }
        public int CategoryId { get; set; }
        public int AppUserId { get; set; }
    }
}
