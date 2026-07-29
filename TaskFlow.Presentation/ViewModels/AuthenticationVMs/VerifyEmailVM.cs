using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Presentation.ViewModels.AuthenticationVMs;

public class VerifyEmailVM
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "OTP is required.")]
    public string Otp { get; set; } = string.Empty;
}