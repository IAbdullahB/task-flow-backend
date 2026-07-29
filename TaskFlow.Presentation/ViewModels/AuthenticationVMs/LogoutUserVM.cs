using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Presentation.ViewModels.AuthenticationVMs;
public class LogoutUserVM
{
    [Required(ErrorMessage = "Token is required.")]
    public string Token { get; set; } = string.Empty;
}
