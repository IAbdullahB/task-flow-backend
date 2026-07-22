namespace TaskFlow.Application.Dtos.AuthenticationDtos;

public class ChangePasswordDto
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}