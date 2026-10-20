using AutoMapper;
using Blog.Areas.blgpnl.Models;
using Blog.Helper;
using BusinessLayer.Abstract;
using DtoLayer.Dtos.UserDtos;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Areas.blgpnl.Controllers;

[Area("blgpnl")]
[Authorize(Roles = "Admin")]
[Route("blgpnl/kullanicilar")]
public class UsersController(
    IAppUserService appUserService,
    UserManager<AppUser> userManager,
    IWebHostEnvironment webHostEnvironment,
    RoleManager<AppRole> roleManager,
    IMapper mapper,
    ILogger<UsersController> logger) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var users = appUserService.TGetList();
        var viewModel = new List<UserWithRolesViewModel>(users.Count);

        foreach (var user in users)
        {
            viewModel.Add(new UserWithRolesViewModel
            {
                User = user,
                Roles = await userManager.GetRolesAsync(user)
            });
        }

        return View(viewModel);
    }

    [HttpGet]
    [Route("guncelle/{id}")]
    public IActionResult Edit(int id)
    {
        if (!IsAuthorizedForUser(id.ToString())) return Forbid();

        var user = appUserService.TGetByID(id);
        if (user is null) return NotFound();

        var dto = mapper.Map<UserEditDto>(user);
        return View(dto);
    }

    [HttpPost]
    [Route("guncelle/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UserEditDto model)
    {
        if (!IsAuthorizedForUser(model.Id.ToString())) return Forbid();

        var user = await userManager.FindByIdAsync(model.Id.ToString());
        if (user is null) return NotFound();

        if (!ModelState.IsValid)
        {
            model.ExistingImageUrl = user.ImageUrl;
            return View(model);
        }

        if (model.ProfileImage is not null && !ImageHelper.IsValidImage(model.ProfileImage, out var imgError))
        {
            ModelState.AddModelError(string.Empty, imgError ?? "Geçersiz profil resmi.");
            model.ExistingImageUrl = user.ImageUrl;
            return View(model);
        }

        user.Name = model.Name;
        user.Surname = model.Surname;

        if (!string.IsNullOrWhiteSpace(model.UserName) && user.UserName != model.UserName)
        {
            var result = await userManager.SetUserNameAsync(user, model.UserName);
            if (!result.Succeeded) return AddErrorsAndReturn(result.Errors, model, user.ImageUrl);
        }

        if (!string.IsNullOrWhiteSpace(model.Email) && user.Email != model.Email)
        {
            var result = await userManager.SetEmailAsync(user, model.Email);
            if (!result.Succeeded) return AddErrorsAndReturn(result.Errors, model, user.ImageUrl);
        }

        if (model.ProfileImage is not null && model.ProfileImage.Length > 0)
        {
            var uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "uploads", "users", "profilepicture");
            var fileName = await ImageHelper.SaveImageAsWebpAsync(model.ProfileImage, uploadFolder, "profile");
            user.ImageUrl = $"/uploads/users/profilepicture/{fileName}";
        }

        var updateResult = await userManager.UpdateAsync(user);
        return updateResult.Succeeded ? Redirect("/blgpnl/kullanicilar") : AddErrorsAndReturn(updateResult.Errors, model, user.ImageUrl);
    }

    [HttpGet]
    [Route("roller/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ManageRoles(int id)
    {
        var user = appUserService.TGetByID(id);
        if (user is null) return NotFound();

        var allRoles = roleManager.Roles.Select(r => r.Name).ToList();
        var userRoles = await userManager.GetRolesAsync(user);

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return Json(new { userId = user.Id, userName = user.UserName, allRoles, userRoles });
        }

        var viewModel = new UserRolesViewModel
        {
            UserId = user.Id,
            UserName = user.UserName,
            Roles = allRoles.Select(r => new RoleCheckboxItem
            {
                RoleName = r,
                IsAssigned = userRoles.Contains(r)
            }).ToList()
        };

        return View(viewModel);
    }

    [HttpPost]
    [Route("roller/{id}")]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ManageRoles(int id, [FromBody] List<string> selectedRoles)
    {
        try
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            if (user is null) return Json(new { success = false, message = "Kullanıcı bulunamadı." });

            selectedRoles = (selectedRoles ?? []).Distinct().ToList();
            var allRoles = roleManager.Roles.Select(r => r.Name).ToList();
            if (selectedRoles.Any(r => !allRoles.Contains(r)))
            {
                return Json(new { success = false, message = "Geçersiz rol seçimi." });
            }

            var currentRoles = await userManager.GetRolesAsync(user);
            if (currentRoles.Contains("Admin") && !selectedRoles.Contains("Admin")
                && (await userManager.GetUsersInRoleAsync("Admin")).Count <= 1)
            {
                return Json(new { success = false, message = "Sistemdeki son yöneticinin Admin rolü kaldırılamaz." });
            }

            var removeResult = await userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                return Json(new { success = false, message = "Mevcut roller kaldırılamadı: " + string.Join(", ", removeResult.Errors.Select(e => e.Description)) });
            }

            if (selectedRoles.Count > 0)
            {
                var addResult = await userManager.AddToRolesAsync(user, selectedRoles);
                if (!addResult.Succeeded)
                {
                    return Json(new { success = false, message = "Rol atanamadı: " + string.Join(", ", addResult.Errors.Select(e => e.Description)) });
                }
            }

            return Json(new { success = true, roles = selectedRoles });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Kullanıcı {UserId} için roller güncellenirken hata oluştu.", id);
            return Json(new { success = false, message = "Roller güncellenirken bir sunucu hatası oluştu." });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("sil/{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteConfirmed(int id)
    {
        var value = appUserService.TGetByID(id);
        if (value is not null)
        {
            appUserService.TDelete(value);
        }
        return RedirectToAction("Index", new { status = 1 });
    }

    private bool IsAuthorizedForUser(string targetUserId) =>
        User.IsInRole("Admin") || userManager.GetUserId(User) == targetUserId;

    private IActionResult AddErrorsAndReturn(IEnumerable<IdentityError> errors, UserEditDto dto, string? existingImageUrl)
    {
        foreach (var error in errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
        dto.ExistingImageUrl = existingImageUrl;
        return View("Edit", dto);
    }
}
