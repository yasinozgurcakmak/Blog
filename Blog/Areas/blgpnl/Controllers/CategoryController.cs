using AutoMapper;
using BusinessLayer.Abstract;
using DtoLayer.Dtos.CategoryDtos;
using EntityLayer.Concrete;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Blog.Areas.blgpnl.Controllers;

[Area("blgpnl")]
[Authorize(Roles = "Admin, Editor")]
[Route("blgpnl/kategoriler")]
public class CategoryController(
    ICategoryService categoryService,
    IMapper mapper,
    IValidator<CreateCategoryDto> categoryValidator,
    ILogger<CategoryController> logger) : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        var categories = categoryService.TGetList();
        var dtoList = mapper.Map<List<CategoryListDto>>(categories);
        return View(dtoList);
    }

    [HttpGet("ekle")]
    public IActionResult AddCategory() => View(new CreateCategoryDto());

    [HttpPost("ekle")]
    [ValidateAntiForgeryToken]
    public IActionResult AddCategory(CreateCategoryDto dto)
    {
        var validationResult = categoryValidator.Validate(dto);
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
            return View(dto);
        }

        try
        {
            var category = mapper.Map<Category>(dto);
            categoryService.TInsert(category);
            return Redirect("/blgpnl/kategoriler");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Kategori eklenirken hata oluştu.");
            ModelState.AddModelError(string.Empty, "Kategori eklenirken bir hata oluştu.");
            return View(dto);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("sil/{id}")]
    public IActionResult DeleteCategory(int id)
    {
        var item = categoryService.TGetByID(id);
        if (item is not null)
        {
            categoryService.TDelete(item);
        }
        return RedirectToAction("Index");
    }
}
