namespace TaskFlow.Application.Dtos.TaskItemDtos;
public record GetTaskByIdDto(Guid RequesterUserId, Guid TaskId);