using System.Security.Claims;
using BusinessLayer.Abstract;
using DtoLayer.Dtos.CommentDtos;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Blog.Controllers;

public class CommentController(ICommentService commentService, IValidator<CreateCommentDto> commentValidator) : Controller
{
    private const string CommentsFragment = "comments";

    [HttpPost("/makale/{slug}/yorum")]
    [EnableRateLimiting("write")]
    [ValidateAntiForgeryToken]
    public IActionResult Create(string slug, CreateCommentDto dto)
    {
        if (GetUserId() is not int userId) return RedirectToLogin(slug);

        var validationResult = commentValidator.Validate(dto);
        if (!validationResult.IsValid)
        {
            TempData["CommentError"] = validationResult.Errors[0].ErrorMessage;
            return RedirectToArticle(slug, CommentsFragment);
        }

        var commentId = commentService.AddComment(dto, userId);
        if (commentId is null)
        {
            TempData["CommentError"] = "Yorum gönderilemedi. Cevap verdiğin yorum silinmiş olabilir; sayfayı yenileyip tekrar dene.";
            return RedirectToArticle(slug, CommentsFragment);
        }

        return RedirectToArticle(slug, $"comment-{commentId}");
    }

    [HttpPost("/makale/{slug}/yorum/{id:int}/sil")]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(string slug, int id)
    {
        if (GetUserId() is not int userId) return RedirectToLogin(slug);

        if (!commentService.DeleteComment(id, userId))
        {
            TempData["CommentError"] = "Yorum silinemedi. Yalnızca kendi yorumlarını silebilirsin.";
        }
        return RedirectToArticle(slug, CommentsFragment);
    }

    [HttpPost("/yorum/{id:int}/begen")]
    [EnableRateLimiting("write")]
    [ValidateAntiForgeryToken]
    public IActionResult ToggleLike(int id)
    {
        if (GetUserId() is not int userId) return Unauthorized();

        var result = commentService.ToggleLike(id, userId);
        if (result is null) return NotFound();

        return Json(result);
    }

    private IActionResult RedirectToArticle(string slug, string fragment) =>
        RedirectToAction("Index", "Blog", new { slug }, fragment);

    private IActionResult RedirectToLogin(string slug) =>
        Redirect("/giris-yap?geldigiSayfa=" + Uri.EscapeDataString("/makale/" + slug));

    private int? GetUserId() => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
}
