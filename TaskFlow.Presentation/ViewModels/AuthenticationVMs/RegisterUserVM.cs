using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Presentation.ViewModels.AuthenticationVMs;

public class RegisterUserVM
{
    [Required(ErrorMessage = "Username is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters long.")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
    [RegularExpression(@"^(?=.*[!@#$%^&*~()_+=\-[\]{};:<>|./?])((?=.*\d)).+$",
        ErrorMessage = "Password must contain at least one special character and one digit.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    
}