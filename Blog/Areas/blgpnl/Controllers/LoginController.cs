using DtoLayer.Dtos.UserDtos;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Areas.blgpnl.Controllers;

[Area("blgpnl")]
[AllowAnonymous]
public class LoginController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager) : Controller
{
    [HttpGet]
    [Route("blgpnl/giris-yap")]
    public IActionResult Index() => View(new UserLoginDto());

    [HttpPost]
    [Route("blgpnl/giris-yap")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(UserLoginDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        var userNameOrEmail = dto.UserName.Trim();
        var user = await userManager.FindByEmailAsync(userNameOrEmail) ?? await userManager.FindByNameAsync(userNameOrEmail);
        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Kullanıcı adı/e-posta veya şifre hatalı.");
            return View(dto);
        }

        var roles = await userManager.GetRolesAsync(user);
        var isAuthorized = roles.Any(r => r.Equals("Admin", StringComparison.OrdinalIgnoreCase) || r.Equals("Editor", StringComparison.OrdinalIgnoreCase));
        if (!isAuthorized)
        {
            ModelState.AddModelError(string.Empty, $"Bu panele erişim yetkiniz bulunmamaktadır. (Mevcut roller: {string.Join(", ", roles.DefaultIfEmpty("Yok"))})");
            return View(dto);
        }

        var result = await signInManager.PasswordSignInAsync(user, dto.Password, isPersistent: false, lockoutOnFailure: true);

        if (result.Succeeded) return Redirect("/blgpnl/kullanicilar");

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "Hesabınız geçici olarak kilitlendi. Lütfen daha sonra tekrar deneyin.");
            return View(dto);
        }

        ModelState.AddModelError(string.Empty, "Kullanıcı adı/e-posta veya şifre hatalı.");
        return View(dto);
    }

    [HttpGet]
    [Route("blgpnl/cikis")]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return Redirect("/blgpnl/giris-yap");
    }
}
