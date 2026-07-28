namespace TaskFlow.Application.Dtos.TaskItemDtos;
public record CreateNewTaskDto(
    Guid CreatorUserId,
    Guid AssignedUserId,
    string Title,
    string Description,
    DateTime DueDate);
