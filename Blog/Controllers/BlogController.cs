using System.Diagnostics;
using System.Security.Claims;
using Blog.Models;
using BusinessLayer.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Blog.Controllers
{
    public class BlogController(IBlogService blogService, ICommentService commentService) : Controller
    {
        private static readonly TimeSpan ViewCookieLifetime = TimeSpan.FromHours(24);

        [HttpGet("/makale/{slug}")]
        public IActionResult Index(string slug)
        {
            var cookieName = $"bv_{slug}";
            var countView = !Request.Cookies.ContainsKey(cookieName);
            var userId = GetUserId();

            var blog = blogService.GetBlog(slug, countView, userId);
            if (blog is null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }

            (blog.Comments, blog.CommentCount) = commentService.GetCommentTree(blog.Id, userId);

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

        [HttpPost("/makale/{id:int}/alkis")]
        [EnableRateLimiting("write")]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleLike(int id)
        {
            if (GetUserId() is not int userId) return Unauthorized();

            var result = blogService.ToggleLike(id, userId);
            if (result is null) return NotFound();

            return Json(result);
        }

        [HttpPost("/makale/{id:int}/kaydet")]
        [EnableRateLimiting("write")]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleBookmark(int id)
        {
            if (GetUserId() is not int userId) return Unauthorized();

            var bookmarked = blogService.ToggleBookmark(id, userId);
            if (bookmarked is null) return NotFound();

            return Json(new { bookmarked });
        }

        [HttpGet("/kaydedilenler")]
        public IActionResult Bookmarks()
        {
            if (GetUserId() is not int userId) return Redirect("/giris-yap?geldigiSayfa=" + Uri.EscapeDataString("/kaydedilenler"));

            return View("~/Views/pages/Bookmarks.cshtml", blogService.GetBookmarkedBlogCards(userId));
        }

        private int? GetUserId() => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
    }
}
