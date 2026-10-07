namespace TaskFlow.Application.Dtos.RequestDtos.TaskItemDtos;
public record GetTaskByIdDto(Guid RequesterUserId, Guid TaskId);