using BusinessLayer.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    public class BlogController(IBlogService blogService) : Controller
    {
        private static readonly TimeSpan ViewCookieLifetime = TimeSpan.FromHours(24);

        [HttpGet("/makale/{slug}")]
        public IActionResult Index(string slug)
        {
            var cookieName = $"bv_{slug}";
            var countView = !Request.Cookies.ContainsKey(cookieName);

            var blog = blogService.GetBlog(slug, countView);
            if (blog is null) return NotFound();

            if (countView)
            {
                Response.Cookies.Append(cookieName, "1", new CookieOptions
                {
                    MaxAge = ViewCookieLifetime,
                    HttpOnly = true,
                    Secure = Request.IsHttps,
                    SameSite = SameSiteMode.Lax,
                    Path = "/makale"
                });
            }

            return View("~/Views/pages/BlogDetail.cshtml", blog);
        }
    }
}
