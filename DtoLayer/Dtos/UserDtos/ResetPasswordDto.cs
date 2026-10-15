namespace DtoLayer.Dtos.UserDtos;

public class ResetPasswordDto
{
    public int UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}
