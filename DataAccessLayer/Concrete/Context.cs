
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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<BlogLike>(e =>
            {
                e.HasIndex(l => new { l.BlogId, l.AppUserId }).IsUnique();
                e.HasOne<Blog>().WithMany().HasForeignKey(l => l.BlogId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne<AppUser>().WithMany().HasForeignKey(l => l.AppUserId).OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
