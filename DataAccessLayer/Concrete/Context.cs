
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
    }
}
