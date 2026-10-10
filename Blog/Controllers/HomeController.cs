using System.Diagnostics;
using Blog.Models;
using BusinessLayer.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers;

public class HomeController(IBlogService blogService, ICategoryService categoryService) : Controller
{
    private const int PageSize = 3;
    private const int SidebarCategoryCount = 5;

    [HttpGet]
    public IActionResult Index(int? categoryId)
    {
        var page = blogService.GetBlogCards(categoryId, page: 1, pageSize: PageSize);
        var vm = new HomeIndexViewModel
        {
            Blogs = page.Items,
            HasMore = page.HasMore,
            Categories = categoryService.GetPopularCategories(SidebarCategoryCount),
            SelectedCategoryId = categoryId
        };
        return View(vm);
    }

    [HttpGet]
    public IActionResult LoadMore(int page, int? categoryId)
    {
        if (page < 2) return BadRequest();

        var result = blogService.GetBlogCards(categoryId, page, PageSize);
        Response.Headers["X-Has-More"] = result.HasMore ? "true" : "false";
        return PartialView("_BlogCards", result.Items);
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
