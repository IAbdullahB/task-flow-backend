namespace TaskFlow.Application.Dtos.TaskItemDtos;
public class DeleteTaskDto
{
    public Guid RequesterUserId { get; set; }
    public Guid TaskId { get; set; }
}
