namespace TaskFlow.Application.Dtos.RequestDtos.TaskItemDtos;
public record ReassignTaskDto(Guid RequesterUserId, Guid TaskId, Guid NewAssignedUserId);