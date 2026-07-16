using Microsoft.AspNetCore.Identity;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Domain.Entities;
public class User
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string HashPassword { get; set; } = string.Empty;
    public string SaltPassword { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public ICollection<TaskItem> TaskItems { get; set; } = [];
}
