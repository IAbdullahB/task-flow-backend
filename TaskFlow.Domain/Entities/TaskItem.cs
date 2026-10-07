
namespace TaskFlow.Domain.Entities;
public class TaskItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsDone { get; set; } = false;
    public DateTime? DueDate { get; set; }
    public Guid? AssignedUserId { get; set; }
    public User AssignedUser { get; set; } = null!;
}
