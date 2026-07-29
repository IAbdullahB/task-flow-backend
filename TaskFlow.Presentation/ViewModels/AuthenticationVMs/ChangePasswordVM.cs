using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Presentation.ViewModels.AuthenticationVMs;

public class ChangePasswordVM
{
    [Required(ErrorMessage = "Current password is required.")]
    [DataType(DataType.Password)]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "New password is required.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "New password must be at least 6 characters long.")]
    [RegularExpression(@"^(?=.*[!@#$%^&*~()_+=\-[\]{};:<>|./?])((?=.*\d)).+$",
        ErrorMessage = "Password must contain at least one special character and one digit.")]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; } = string.Empty;

    
}