using System.Text;
using System.Text.Encodings.Web;
using Blog.Services;
using DtoLayer.Dtos.UserDtos;
using EntityLayer.Concrete;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;

namespace Blog.Controllers;

public class PasswordController(
    UserManager<AppUser> userManager,
    IEmailService emailService,
    IMemoryCache cache,
    IConfiguration configuration,
    IWebHostEnvironment environment,
    IValidator<ForgotPasswordDto> forgotValidator,
    IValidator<ResetPasswordDto> resetValidator,
    ILogger<PasswordController> logger) : Controller
{
    private const string ForgotView = "~/Views/pages/ForgotPassword.cshtml";
    private const string ResetView = "~/Views/pages/ResetPassword.cshtml";

    
    public const int ResendCooldownSeconds = 60;

    [HttpGet("/sifremi-unuttum")]
    public IActionResult Forgot()
    {
        if (User.Identity?.IsAuthenticated == true) return Redirect(AppUrl("/"));

        ViewData["SentTo"] = TempData["ResetSentTo"] as string;
        return View(ForgotView, new ForgotPasswordDto());
    }

    [HttpPost("/sifremi-unuttum")]
    [EnableRateLimiting("auth")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Forgot(ForgotPasswordDto dto)
    {
        dto.Email = dto.Email?.Trim() ?? string.Empty;

        var validationResult = forgotValidator.Validate(dto);
        ModelState.Clear();
        foreach (var error in validationResult.Errors) ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        if (!validationResult.IsValid) return View(ForgotView, dto);

        
        var cooldownKey = "pwd-reset:" + dto.Email.ToUpperInvariant();
        if (!cache.TryGetValue(cooldownKey, out _))
        {
            cache.Set(cooldownKey, true, TimeSpan.FromSeconds(ResendCooldownSeconds));

            var user = await userManager.FindByEmailAsync(dto.Email);
            if (user is not null)
            {
                try
                {
                    await SendResetEmailAsync(user);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Şifre sıfırlama e-postası gönderilemedi. Kullanıcı: {UserId}", user.Id);
                    cache.Remove(cooldownKey);
                    ModelState.AddModelError(string.Empty, "E-posta şu anda gönderilemedi. Lütfen biraz sonra tekrar dene.");
                    return View(ForgotView, dto);
                }
            }
        }

        TempData["ResetSentTo"] = dto.Email;
        return Redirect(AppUrl("/sifremi-unuttum"));
    }

    [HttpGet("/sifre-sifirla")]
    public async Task<IActionResult> Reset(int uid, string? token)
    {
        var dto = new ResetPasswordDto { UserId = uid, Token = token ?? string.Empty };
        ViewData["LinkValid"] = await IsTokenValidAsync(dto);
        return View(ResetView, dto);
    }

    [HttpPost("/sifre-sifirla")]
    [EnableRateLimiting("auth")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reset(ResetPasswordDto dto)
    {
        var user = await userManager.FindByIdAsync(dto.UserId.ToString());
        var decodedToken = DecodeToken(dto.Token);
        if (user is null || decodedToken is null)
        {
            ViewData["LinkValid"] = false;
            return View(ResetView, dto);
        }

        ViewData["LinkValid"] = true;
        var validationResult = resetValidator.Validate(dto);
        ModelState.Clear();
        foreach (var error in validationResult.Errors) ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        if (!validationResult.IsValid) return View(ResetView, dto);

        var result = await userManager.ResetPasswordAsync(user, decodedToken, dto.Password);
        if (!result.Succeeded)
        {
            if (result.Errors.Any(e => e.Code == "InvalidToken"))
            {
                ViewData["LinkValid"] = false;
                return View(ResetView, dto);
            }
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
            return View(ResetView, dto);
        }

        await userManager.ResetAccessFailedCountAsync(user);
        await userManager.SetLockoutEndDateAsync(user, null);

        TempData["LoginSuccess"] = "Şifren başarıyla güncellendi. Yeni şifrenle giriş yapabilirsin.";
        return Redirect(AppUrl("/giris-yap"));
    }

    private string AppUrl(string path) => $"{Request.PathBase}{path}";

    private async Task SendResetEmailAsync(AppUser user)
    {
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        var link = $"{GetBaseUrl()}/sifre-sifirla?uid={user.Id}&token={encodedToken}";
        var encodedLink = HtmlEncoder.Default.Encode(link);
        var name = HtmlEncoder.Default.Encode(user.Name);

        var body = $"""
            <div style="font-family:Arial,sans-serif;max-width:520px;margin:0 auto;color:#1e293b">
              <h2 style="color:#4f46e5">Şifre sıfırlama</h2>
              <p>Merhaba {name},</p>
              <p>Hesabın için şifre sıfırlama talebinde bulunuldu. Yeni şifre belirlemek için aşağıdaki butona tıkla:</p>
              <p style="margin:28px 0">
                <a href="{encodedLink}" style="background:#6366f1;color:#fff;padding:12px 22px;border-radius:8px;text-decoration:none;font-weight:bold">Şifremi Sıfırla</a>
              </p>
              <p style="font-size:13px;color:#64748b">Bu link 1 saat geçerlidir ve yalnızca bir kez kullanılabilir. Bu talebi sen yapmadıysan bu e-postayı yok sayabilirsin, şifren değişmez.</p>
              <p style="font-size:12px;color:#94a3b8;word-break:break-all">Buton çalışmazsa bu adresi tarayıcına yapıştır:<br>{encodedLink}</p>
            </div>
            """;

        await emailService.SendAsync(user.Email!, "Şifre sıfırlama talebin", body);
    }

    private string GetBaseUrl()
    {
        var baseUrl = configuration["App:BaseUrl"];
        if (!string.IsNullOrWhiteSpace(baseUrl)) return baseUrl.TrimEnd('/');
        if (environment.IsDevelopment()) return $"{Request.Scheme}://{Request.Host}";

        throw new InvalidOperationException("App:BaseUrl ayarı yapılandırılmamış.");
    }

    private async Task<bool> IsTokenValidAsync(ResetPasswordDto dto)
    {
        var decodedToken = DecodeToken(dto.Token);
        if (decodedToken is null) return false;

        var user = await userManager.FindByIdAsync(dto.UserId.ToString());
        return user is not null && await userManager.VerifyUserTokenAsync(
            user, userManager.Options.Tokens.PasswordResetTokenProvider, UserManager<AppUser>.ResetPasswordTokenPurpose, decodedToken);
    }

    private static string? DecodeToken(string? token)
    {
        if (string.IsNullOrWhiteSpace(token)) return null;
        try
        {
            return Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        }
        catch (FormatException)
        {
            return null;
        }
    }
}
