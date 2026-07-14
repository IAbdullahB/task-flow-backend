using Microsoft.AspNetCore.Identity;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Domain.Entities;
public class User : IdentityUser<Guid>
{
    public UserRole Role { get; set; }

    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
