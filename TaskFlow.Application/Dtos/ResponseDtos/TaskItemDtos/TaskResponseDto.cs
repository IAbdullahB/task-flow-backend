namespace TaskFlow.Application.Dtos.ResponseDtos.TaskItemDtos;
public record TaskResponseDto(
    Guid Id,
    string Title,
    string? Description,
    bool IsDone,
    DateTime? DueDate,
    Guid? AssignedUserId,
    string? AssignedUserName 
);
