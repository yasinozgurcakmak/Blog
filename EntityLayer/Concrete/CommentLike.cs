using System;
using System.ComponentModel.DataAnnotations;

namespace EntityLayer.Concrete
{
    public class CommentLike
    {
        [Key]
        public int Id { get; set; }
        public int CommentId { get; set; }
        public int AppUserId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
