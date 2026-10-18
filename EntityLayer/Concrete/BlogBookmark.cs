using System;
using System.ComponentModel.DataAnnotations;

namespace EntityLayer.Concrete
{
    public class BlogBookmark
    {
        [Key]
        public int Id { get; set; }
        public int BlogId { get; set; }
        public int AppUserId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
