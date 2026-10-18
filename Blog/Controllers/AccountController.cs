using DtoLayer.Dtos.UserDtos;
using EntityLayer.Concrete;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers;

public class AccountController(
    SignInManager<AppUser> signInManager,
    UserManager<AppUser> userManager,
    RoleManager<AppRole> roleManager,
    IValidator<UserRegisterDto> registerValidator,
    IValidator<MemberLoginDto> loginValidator) : Controller
{
    private const string LoginView = "~/Views/pages/Login.cshtml";
    private const string RegisterView = "~/Views/pages/Register.cshtml";
    private const string DefaultRole = "User";

    [HttpGet("/giris-yap")]
    public IActionResult Login(string? geldigiSayfa = null)
    {
        if (User.Identity?.IsAuthenticated == true) return Redirect("/");

        ViewData["geldigiSayfa"] = geldigiSayfa;
        return View(LoginView, new MemberLoginDto());
    }

    [HttpPost("/giris-yap")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(MemberLoginDto dto, string? geldigiSayfa = null)
    {
        ViewData["geldigiSayfa"] = geldigiSayfa;

        if (!AddValidationErrors(loginValidator.Validate(dto))) return View(LoginView, dto);

        var userNameOrEmail = dto.UserNameOrEmail.Trim();
        var user = await userManager.FindByEmailAsync(userNameOrEmail) ?? await userManager.FindByNameAsync(userNameOrEmail);
        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Kullanıcı adı/e-posta veya şifre hatalı.");
            return View(LoginView, dto);
        }

        var result = await signInManager.PasswordSignInAsync(user, dto.Password, dto.RememberMe, lockoutOnFailure: true);

        if (result.Succeeded) return Url.IsLocalUrl(geldigiSayfa) ? Redirect(geldigiSayfa) : Redirect("/");

        ModelState.AddModelError(string.Empty, result.IsLockedOut
            ? "Çok fazla hatalı deneme yaptınız. Hesabınız geçici olarak kilitlendi, lütfen daha sonra tekrar deneyin."
            : "Kullanıcı adı/e-posta veya şifre hatalı.");
        return View(LoginView, dto);
    }

    [HttpGet("/uye-ol")]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true) return Redirect("/");

        return View(RegisterView, new UserRegisterDto());
    }

    [HttpPost("/uye-ol")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(UserRegisterDto dto)
    {
        
        dto.Name = dto.Name?.Trim() ?? string.Empty;
        dto.Surname = dto.Surname?.Trim() ?? string.Empty;
        dto.Email = dto.Email?.Trim() ?? string.Empty;

        if (!AddValidationErrors(registerValidator.Validate(dto))) return View(RegisterView, dto);

        if (await userManager.FindByEmailAsync(dto.Email) is not null)
        {
            ModelState.AddModelError(nameof(dto.Email), "Bu e-posta adresiyle kayıtlı bir hesap zaten var.");
            return View(RegisterView, dto);
        }

        var user = new AppUser
        {
            Name = dto.Name,
            Surname = dto.Surname,
            Email = dto.Email,
            UserName = dto.Email,
            ImageUrl = string.Empty,
            EmailConfirmed = true
        };

        
        var result = await userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                var message = error.Code is "DuplicateUserName" or "DuplicateEmail"
                    ? "Bu e-posta adresiyle kayıtlı bir hesap zaten var."
                    : error.Description;
                ModelState.AddModelError(string.Empty, message);
            }
            return View(RegisterView, dto);
        }

        if (!await roleManager.RoleExistsAsync(DefaultRole))
        {
            await roleManager.CreateAsync(new AppRole { Name = DefaultRole });
        }
        await userManager.AddToRoleAsync(user, DefaultRole);

        await signInManager.SignInAsync(user, isPersistent: false);
        return Redirect("/");
    }

    [HttpPost("/cikis-yap")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return Redirect("/");
    }

    private bool AddValidationErrors(FluentValidation.Results.ValidationResult validationResult)
    {
        
        ModelState.Clear();
        foreach (var error in validationResult.Errors)
        {
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }
        return validationResult.IsValid;
    }
}
