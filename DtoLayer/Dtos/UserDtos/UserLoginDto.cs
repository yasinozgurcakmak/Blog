using System.ComponentModel.DataAnnotations;

namespace DtoLayer.Dtos.UserDtos;

public class UserLoginDto
{
    [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre zorunludur.")]
    public string Password { get; set; } = string.Empty;
}
