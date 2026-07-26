namespace TaskFlow.Application.Dtos.UserDtos;

public record GetUserByIdDto(Guid RequesterId, Guid UserId);