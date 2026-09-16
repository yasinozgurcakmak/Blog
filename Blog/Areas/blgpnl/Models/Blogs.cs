namespace Blog.Areas.blgpnl.Models
{
    public class Blogs
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public string Thumbnail { get; set; }
        public string Slug { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool Status { get; set; }
        public int CategoryId { get; set; }
        public int AppUserId { get; set; }

    }
}
