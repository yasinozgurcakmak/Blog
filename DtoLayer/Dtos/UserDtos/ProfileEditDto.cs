using Microsoft.AspNetCore.Http;

namespace DtoLayer.Dtos.UserDtos;

public class ProfileEditDto
{
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;

    // Yalnızca gösterim için; e-posta aynı zamanda kullanıcı adı olduğundan bu formdan değiştirilmez
    public string? Email { get; set; }

    public string? Headline { get; set; }
    public string? Bio { get; set; }

    public string? ExistingImageUrl { get; set; }
    public IFormFile? ProfileImage { get; set; }
    public bool RemoveImage { get; set; }

    public string? GithubUrl { get; set; }
    public string? InstagramUrl { get; set; }
    public string? XUrl { get; set; }
    public string? FacebookUrl { get; set; }
    public string? WebsiteUrl { get; set; }
}
