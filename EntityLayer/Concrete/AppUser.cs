using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class AppUser : IdentityUser<int>
    {
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? Gender { get; set; }
        public string? Headline { get; set; }
        public string? Bio { get; set; }
        public string? GithubUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public string? XUrl { get; set; }
        public string? FacebookUrl { get; set; }
        public string? WebsiteUrl { get; set; }
    }
}
