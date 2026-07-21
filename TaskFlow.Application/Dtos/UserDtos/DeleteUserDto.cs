namespace TaskFlow.Application.Dtos.UserDtos;

public class DeleteUserDto
{
    public Guid RequesterId { get; set; }
    public Guid UserId { get; set; }
}