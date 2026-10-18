namespace DtoLayer.Dtos.UserDtos;

public class MemberLoginDto
{
    public string UserNameOrEmail { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; } = true;
}
