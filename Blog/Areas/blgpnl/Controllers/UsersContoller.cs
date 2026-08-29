using Blog.Areas.blgpnl.Models;
using BusinessLayer.Abstract;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Blog.Areas.blgpnl.Controllers
{
    [Area("blgpnl")]
    [Authorize(Roles = "Admin, Editor")]
    [Route("blgpnl/kullanicilar")]
    public class UsersController : Controller
    {
        private readonly IAppUserService _appUserService;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UsersController(IAppUserService appUserService, UserManager<AppUser> userManager, IWebHostEnvironment webHostEnvironment, RoleManager<AppRole> roleManager)
        {
            _appUserService = appUserService;
            _userManager = userManager;
            _roleManager = roleManager;
            _webHostEnvironment = webHostEnvironment;
        }


        public async Task<IActionResult> Index()
        {
            var users = _appUserService.TGetList();
            var viewModel = new List<UserWithRolesViewModel>();

            foreach (var user in users)
            {
                viewModel.Add(new UserWithRolesViewModel
                {
                    User = user,
                    Roles = await _userManager.GetRolesAsync(user)
                });
            }

            return View(viewModel);
        }

        [HttpGet]
        [Route("guncelle/{id}")]
        public IActionResult Edit(int id)
        {
            var user = _appUserService.TGetByID(id);
            return user == null ? NotFound() : View(user);
        }

        [HttpPost]
        [Route("guncelle/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AppUser model, IFormFile? ProfileImage)
        {
            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(model.Name)) user.Name = model.Name;
            if (!string.IsNullOrWhiteSpace(model.Surname)) user.Surname = model.Surname;

            if (!string.IsNullOrWhiteSpace(model.UserName) && user.UserName != model.UserName)
            {
                var result = await _userManager.SetUserNameAsync(user, model.UserName);
                if (!result.Succeeded) return AddErrorsAndReturn(result.Errors, user);
            }

            if (!string.IsNullOrWhiteSpace(model.Email) && user.Email != model.Email)
            {
                var result = await _userManager.SetEmailAsync(user, model.Email);
                if (!result.Succeeded) return AddErrorsAndReturn(result.Errors, user);
            }


            if (ProfileImage != null && ProfileImage.Length > 0)
            {
                var uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "users", "profilepicture");
                Directory.CreateDirectory(uploadFolder);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(ProfileImage.FileName)}";
                var filePath = Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfileImage.CopyToAsync(stream);
                }

                user.ImageUrl = $"/uploads/users/profilepicture/{fileName}";
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (updateResult.Succeeded) return Redirect("/blgpnl/kullanicilar");

            return AddErrorsAndReturn(updateResult.Errors, user);
        }

        [HttpGet]
        [Route("roller/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageRoles(int id)
        {
            var user = _appUserService.TGetByID(id);
            if (user == null) return NotFound();

            var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();
            var userRoles = await _userManager.GetRolesAsync(user);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new
                {
                    userId = user.Id,
                    userName = user.UserName,
                    allRoles,
                    userRoles
                });
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
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user == null) return Json(new { success = false, message = "Kullanıcı bulunamadı." });

                var currentRoles = await _userManager.GetRolesAsync(user);
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                    return Json(new { success = false, message = "Mevcut roller kaldırılamadı: " + string.Join(", ", removeResult.Errors.Select(e => e.Description)) });

                if (selectedRoles != null && selectedRoles.Any())
                {
                    var addResult = await _userManager.AddToRolesAsync(user, selectedRoles);
                    if (!addResult.Succeeded)
                        return Json(new { success = false, message = "Rol atanamadı: " + string.Join(", ", addResult.Errors.Select(e => e.Description)) });
                }

                return Json(new { success = true, roles = selectedRoles ?? new List<string>() });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Sunucu hatası: " + ex.Message });
            }
        }


        [HttpGet]
        [Route("sil/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteConfirmed(int id)
        {
            var value = _appUserService.TGetByID(id);
            if (value != null)
            {
                _appUserService.TDelete(value);
            }

            return RedirectToAction("Index", new { status = 1 });
        }

        private IActionResult AddErrorsAndReturn(IEnumerable<IdentityError> errors, AppUser user)
        {
            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View("Edit", user);
        }
    }
}