using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Presentation.ViewModels.AuthenticationVMs;

public class RequestPasswordResetVM
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; } = string.Empty;
}