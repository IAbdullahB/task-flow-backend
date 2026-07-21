namespace TaskFlow.Application.Dtos.TaskItemDtos;
public class CreateNewTaskDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public Guid? AssignedUserId { get; set; }
    public Guid CreatorUserId { get; set; }

}
