namespace TaskFlow.Application.Dtos.TaskItemDtos;
public class ReassignTaskDto
{
    public Guid RequesterUserId { get; set; }
    public Guid TaskId { get; set; }
    public Guid NewAssignedUserId { get; set; }
}