namespace TaskFlow.Application.Dtos.TaskItemDtos;
public record ReassignTaskDto(Guid RequesterUserId, Guid TaskId, Guid NewAssignedUserId);