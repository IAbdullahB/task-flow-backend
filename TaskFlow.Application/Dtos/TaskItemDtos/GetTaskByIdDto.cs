namespace TaskFlow.Application.Dtos.TaskItemDtos;
public class GetTaskByIdDto
{
    public Guid RequesterUserId { get; set; }
    public Guid TaskId { get; set; }
}