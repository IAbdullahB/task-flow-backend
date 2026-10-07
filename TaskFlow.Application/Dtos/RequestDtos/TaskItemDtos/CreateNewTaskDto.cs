namespace TaskFlow.Application.Dtos.RequestDtos.TaskItemDtos;
public record CreateNewTaskDto(
    Guid CreatorUserId,
    Guid? AssignedUserId,
    string Title,
    string Description,
    DateTime DueDate);
