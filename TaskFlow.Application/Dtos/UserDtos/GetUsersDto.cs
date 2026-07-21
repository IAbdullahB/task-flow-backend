namespace TaskFlow.Application.Dtos.UserDtos;

public class GetUsersDto
{
    public Guid RequesterId { get; set; }
    public Guid UserId { get; set; }
}