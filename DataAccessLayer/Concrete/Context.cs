
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EntityLayer.Concrete;

namespace DataAccessLayer.Concrete
{
    public class Context : IdentityDbContext<AppUser, AppRole, int>
    {
        public Context(DbContextOptions<Context> options) : base(options) { }
        public DbSet<Category> Category { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<BlogLike> BlogLikes { get; set; }
        public DbSet<BlogBookmark> BlogBookmarks { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CommentLike> CommentLikes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<BlogLike>(e =>
            {
                e.HasIndex(l => new { l.BlogId, l.AppUserId }).IsUnique();
                e.HasOne<Blog>().WithMany().HasForeignKey(l => l.BlogId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne<AppUser>().WithMany().HasForeignKey(l => l.AppUserId).OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<BlogBookmark>(e =>
            {
                e.HasIndex(b => new { b.AppUserId, b.BlogId }).IsUnique();
                e.HasOne<Blog>().WithMany().HasForeignKey(b => b.BlogId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne<AppUser>().WithMany().HasForeignKey(b => b.AppUserId).OnDelete(DeleteBehavior.Cascade);
            });


            builder.Entity<Comment>(e =>
            {
                e.Property(c => c.Content).HasMaxLength(1000);
                e.HasOne<Blog>().WithMany().HasForeignKey(c => c.BlogId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne<AppUser>().WithMany().HasForeignKey(c => c.AppUserId).OnDelete(DeleteBehavior.NoAction);
                e.HasOne<Comment>().WithMany().HasForeignKey(c => c.ParentCommentId).OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<CommentLike>(e =>
            {
                e.HasIndex(l => new { l.CommentId, l.AppUserId }).IsUnique();
                e.HasOne<Comment>().WithMany().HasForeignKey(l => l.CommentId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne<AppUser>().WithMany().HasForeignKey(l => l.AppUserId).OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}
