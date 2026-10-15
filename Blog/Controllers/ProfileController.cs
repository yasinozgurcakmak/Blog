using Blog.Helper;
using DtoLayer.Dtos.UserDtos;
using EntityLayer.Concrete;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers;

public class ProfileController(
    SignInManager<AppUser> signInManager,
    UserManager<AppUser> userManager,
    IValidator<ProfileEditDto> profileValidator,
    IWebHostEnvironment webHostEnvironment) : Controller
{
    private const string EditView = "~/Views/pages/EditProfile.cshtml";
    private const string PageUrl = "profili-guncelle";
    private const long MaxImageSizeBytes = 2 * 1024 * 1024;

    [HttpGet(PageUrl)]
    public async Task<IActionResult> Edit()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null) return Redirect($"/giris-yap?geldigiSayfa={Uri.EscapeDataString(PageUrl)}");

        return View(EditView, new ProfileEditDto
        {
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email,
            Headline = user.Headline,
            Bio = user.Bio,
            ExistingImageUrl = user.ImageUrl,
            GithubUrl = user.GithubUrl,
            InstagramUrl = user.InstagramUrl,
            XUrl = user.XUrl,
            FacebookUrl = user.FacebookUrl,
            WebsiteUrl = user.WebsiteUrl
        });
    }

    [HttpPost(PageUrl)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProfileEditDto dto)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null) return Redirect($"/giris-yap?geldigiSayfa={Uri.EscapeDataString(PageUrl)}");

        dto.Email = user.Email;
        dto.ExistingImageUrl = user.ImageUrl;
        dto.Name = dto.Name?.Trim() ?? string.Empty;
        dto.Surname = dto.Surname?.Trim() ?? string.Empty;
        dto.Headline = string.IsNullOrWhiteSpace(dto.Headline) ? null : dto.Headline.Trim();
        dto.Bio = string.IsNullOrWhiteSpace(dto.Bio) ? null : dto.Bio.Replace("\r\n", "\n").Trim();
        dto.GithubUrl = NormalizeUrl(dto.GithubUrl);
        dto.InstagramUrl = NormalizeUrl(dto.InstagramUrl);
        dto.XUrl = NormalizeUrl(dto.XUrl);
        dto.FacebookUrl = NormalizeUrl(dto.FacebookUrl);
        dto.WebsiteUrl = NormalizeUrl(dto.WebsiteUrl);

        var validationResult = profileValidator.Validate(dto);
        ModelState.Clear();
        foreach (var error in validationResult.Errors)
        {
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }

        if (!ImageHelper.IsValidImage(dto.ProfileImage, out var imageError, MaxImageSizeBytes))
        {
            ModelState.AddModelError(nameof(dto.ProfileImage), imageError ?? "Geçersiz profil resmi.");
        }

        if (!ModelState.IsValid) return View(EditView, dto);

        user.Name = dto.Name;
        user.Surname = dto.Surname;
        user.Headline = dto.Headline;
        user.Bio = dto.Bio;
        user.GithubUrl = dto.GithubUrl;
        user.InstagramUrl = dto.InstagramUrl;
        user.XUrl = dto.XUrl;
        user.FacebookUrl = dto.FacebookUrl;
        user.WebsiteUrl = dto.WebsiteUrl;

        if (dto.ProfileImage is { Length: > 0 })
        {
            var uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "uploads", "users", "profilepicture");
            var fileName = await ImageHelper.SaveImageAsWebpAsync(dto.ProfileImage, uploadFolder, "profile");
            user.ImageUrl = $"/uploads/users/profilepicture/{fileName}";
        }
        else if (dto.RemoveImage)
        {
            user.ImageUrl = string.Empty;
        }

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
            return View(EditView, dto);
        }

        await signInManager.RefreshSignInAsync(user);

        TempData["ProfileSaved"] = "Profil bilgilerin başarıyla kaydedildi.";
        return Redirect(PageUrl);
    }

    private static string? NormalizeUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return null;

        url = url.Trim();
        return url.Contains("://") ? url : "https://" + url;
    }
}
