using AutoMapper;
using Blog.Areas.blgpnl.Models;
using Blog.Helper;
using BusinessLayer.Abstract;
using DtoLayer.Dtos.BlogDtos;
using EntityLayer.Concrete;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
namespace Blog.Areas.blgpnl.Controllers;

[Area("blgpnl")]
[Authorize(Roles = "Admin, Editor")]
[Route("/blgpnl/yazilar")]
public class BlogController(
    IBlogService blogService,
    ICategoryService categoryService,
    UserManager<AppUser> userManager,
    IWebHostEnvironment webHostEnvironment,
    IMapper mapper,
    IValidator<CreateBlogDto> createBlogValidator,
    IValidator<UpdateBlogDto> updateBlogValidator) : Controller
{
    private static readonly CultureInfo TurkishCulture = new("tr-TR");

    [HttpGet]
    public IActionResult Index()
    {
        var blogs = blogService.TGetList().OrderByDescending(x => x.CreatedDate).ToList();
        var dtoList = mapper.Map<List<BlogListDto>>(blogs);
        return View(dtoList);
    }

    [HttpGet]
    [Route("ara")]
    public IActionResult Search(string? q)
    {
        var searchTerm = q?.Trim();
        var values = string.IsNullOrWhiteSpace(searchTerm)
            ? blogService.TGetList()
            : blogService.TGetListByFilter(x => x.Title.Contains(searchTerm));

        var result = values.OrderByDescending(x => x.CreatedDate).Select(x => new
        {
            id = x.Id,
            title = x.Title,
            slug = x.Slug,
            dateFormatted = x.CreatedDate.ToString("dd MMMM yyyy", TurkishCulture) + (x.UpdatedDate.HasValue ? " / " + x.UpdatedDate.Value.ToString("dd MMMM yyyy", TurkishCulture) : string.Empty),
            statusText = !x.Status ? "Yayında Değil" : (x.PublishDate.HasValue && x.PublishDate.Value > DateTime.Now ? $"Zamanlandı ({x.PublishDate.Value:dd.MM.yyyy HH:mm})" : "Yayında")
        });

        return Json(result);
    }

    [HttpGet]
    [Route("ekle")]
    public IActionResult Add()
    {
        var vm = new BlogCreateViewModel { Dto = new CreateBlogDto() };
        vm.Categories = BuildCategoryList();
        return View(vm);
    }

    [HttpPost]
    [Route("ekle")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add([Bind(Prefix = "Dto")] CreateBlogDto dto)
    {
        var validationResult = createBlogValidator.Validate(dto);
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                ModelState.AddModelError($"Dto.{error.PropertyName}", error.ErrorMessage);
            }
            var vm = new BlogCreateViewModel { Dto = dto };
            vm.Categories = BuildCategoryList(dto.CategoryId);
            return View(vm);
        }

        var finalSlug = ResolveSlug(dto.Slug, dto.Title);
        var uploadedThumbnail = await ProcessThumbnailAsync(dto.ThumbnailFile, finalSlug);

        var blog = mapper.Map<EntityLayer.Concrete.Blog>(dto);
        blog.Slug = finalSlug;
        blog.Thumbnail = uploadedThumbnail ?? string.Empty;
        blog.Content = ProcessContent(dto.Content, finalSlug);
        blog.ReadingTime = ReadingTimeHelper.CalculateReadingTime(blog.Content);
        blog.CreatedDate = DateTime.Now;
        blog.UpdatedDate = null;

        var user = await userManager.GetUserAsync(User);
        if (user is not null)
        {
            blog.AppUserId = user.Id;
        }

        blogService.TInsert(blog);
        return Redirect("/blgpnl/yazilar");
    }

    [HttpPost]
    [Route("upload-editor-image")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> UploadEditorImage(IFormFile file, string? slug)
    {
        if (file is null || file.Length == 0)
        {
            return Json(new { success = false, message = "Geçersiz veya boş dosya gönderildi." });
        }

        if (!ImageHelper.IsValidImage(file, out var imgError))
        {
            return Json(new { success = false, message = imgError ?? "Geçersiz görsel dosyası." });
        }

        var safeSlug = !string.IsNullOrWhiteSpace(slug) ? GenerateSlugHelper.GenerateSlug(slug) : "editor";
        var blogFolder = Path.Combine(webHostEnvironment.WebRootPath, "uploads", "blogs", safeSlug);
        var imageName = await ImageHelper.SaveImageAsWebpAsync(file, blogFolder, "content");

        return Json(new { success = true, url = $"/uploads/blogs/{safeSlug}/{imageName}" });
    }

    [HttpGet]
    [Route("duzenle/{slug}")]
    public IActionResult Details(string slug)
    {
        var value = blogService.TGetListByFilter(x => x.Slug == slug).FirstOrDefault();
        if (value is null) return NotFound();

        var dto = mapper.Map<UpdateBlogDto>(value);
        var vm = new BlogEditViewModel { Dto = dto };
        vm.Categories = BuildCategoryList(dto.CategoryId);
        return View(vm);
    }

    [HttpPost]
    [Route("duzenle/{slug}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Details([Bind(Prefix = "Dto")] UpdateBlogDto dto)
    {
        var existingBlog = blogService.TGetByID(dto.Id);
        if (existingBlog is null) return NotFound();

        var validationResult = updateBlogValidator.Validate(dto);
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                ModelState.AddModelError($"Dto.{error.PropertyName}", error.ErrorMessage);
            }
            var vm = new BlogEditViewModel { Dto = dto };
            vm.Categories = BuildCategoryList(dto.CategoryId);
            return View(vm);
        }

        var finalSlug = ResolveSlug(dto.Slug, dto.Title);

        mapper.Map(dto, existingBlog);
        existingBlog.Slug = finalSlug;

        if (dto.NewThumbnailFile is not null && dto.NewThumbnailFile.Length > 0)
        {
            var uploadedThumbnail = await ProcessThumbnailAsync(dto.NewThumbnailFile, finalSlug);
            if (uploadedThumbnail is not null)
            {
                existingBlog.Thumbnail = uploadedThumbnail;
            }
        }
        else if (!string.IsNullOrWhiteSpace(dto.ExistingThumbnail))
        {
            existingBlog.Thumbnail = dto.ExistingThumbnail;
        }

        existingBlog.Content = ProcessContent(dto.Content, finalSlug);
        existingBlog.ReadingTime = ReadingTimeHelper.CalculateReadingTime(dto.Content);
        existingBlog.UpdatedDate = DateTime.Now;

        blogService.TUpdate(existingBlog);
        return Redirect("/blgpnl/yazilar");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("sil/{id}")]
    public IActionResult Delete(int id)
    {
        var item = blogService.TGetByID(id);
        if (item is not null)
        {
            blogService.TDelete(item);
        }
        return RedirectToAction("Index");
    }

    private static string ResolveSlug(string? slug, string title) => !string.IsNullOrWhiteSpace(slug) ? GenerateSlugHelper.GenerateSlug(slug) : GenerateSlugHelper.GenerateSlug(title);

    private async Task<string?> ProcessThumbnailAsync(IFormFile? thumbnailFile, string slug)
    {
        if (thumbnailFile is null || thumbnailFile.Length == 0) return null;

        var blogFolder = Path.Combine(webHostEnvironment.WebRootPath, "uploads", "blogs", slug);
        var imageName = await ImageHelper.SaveImageAsWebpAsync(thumbnailFile, blogFolder, "thumbnail");
        return $"/uploads/blogs/{slug}/{imageName}";
    }

    private string ProcessContent(string? content, string slug)
    {
        if (string.IsNullOrWhiteSpace(content)) return string.Empty;

        var sanitized = HtmlSanitizerHelper.Sanitize(content);
        return ProcessContentBase64ImagesHelper.ProcessContentBase64Images(sanitized, slug, webHostEnvironment.WebRootPath);
    }

    
    private IEnumerable<SelectListItem> BuildCategoryList(int? selectedId = null) => new SelectList(categoryService.TGetList(), "Id", "Name", selectedId);
}
