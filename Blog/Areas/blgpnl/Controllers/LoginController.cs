using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Blog.Areas.blgpnl.Controllers
{
    [Area("blgpnl")]
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public LoginController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("blgpnl/giris-yap")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("blgpnl/giris-yap")]
        public async Task<IActionResult> Index(string username, string password)
        {
            // 1. Kullanıcıyı bul
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı.");
                return View();
            }

            // 2. Rolünü kontrol et — sadece Admin ve Editor girebilir
            var roles = await _userManager.GetRolesAsync(user);
            bool yetkiliRol = roles.Any(r => r.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                           || roles.Any(r => r.Equals("Editor", StringComparison.OrdinalIgnoreCase));
            if (!yetkiliRol)
            {
                ModelState.AddModelError("", $"Bu panele erişim yetkiniz bulunmamaktadır. (Mevcut roller: {string.Join(", ", roles.Any() ? roles : new[] { "Yok" })})");
                return View();
            }

            // 3. Şifreyi doğrula ve oturum aç
            var result = await _signInManager.PasswordSignInAsync(
                user,
                password,
                isPersistent: false,   // "Beni hatırla" — false = tarayıcı kapanınca oturum biter
                lockoutOnFailure: true // 5 başarısız denemede hesabı kilitle
            );

            if (result.Succeeded)
            {
                return Redirect("/blgpnl/kullanicilar");
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Hesabınız geçici olarak kilitlendi. Lütfen daha sonra tekrar deneyin.");
                return View();
            }

            ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı.");
            return View();
        }

        [HttpGet]
        [Route("blgpnl/cikis")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/blgpnl/giris-yap");
        }
    }
}
