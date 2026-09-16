using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DtoLayer.Dtos.UserDtos;

public class UserEditDto
{
    [Required]
    public int Id { get; set; }

    [Required(ErrorMessage = "Ad alanı zorunludur.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Soyad alanı zorunludur.")]
    public string Surname { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-posta adresi zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    public string Email { get; set; } = string.Empty;

    public string? ExistingImageUrl { get; set; }

    public IFormFile? ProfileImage { get; set; }
}
