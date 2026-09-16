using System.ComponentModel.DataAnnotations;

namespace DtoLayer.Dtos.CategoryDtos;

public class CreateCategoryDto
{
    [Required(ErrorMessage = "Kategori adı zorunludur.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Kategori adı 2 ile 100 karakter arasında olmalıdır.")]
    public string Name { get; set; } = string.Empty;
}
