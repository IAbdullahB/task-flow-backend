namespace TaskFlow.Application.Dtos.UserDtos;

public class GetUserByIdDto
{
    public Guid RequesterId { get; set; }
    public Guid UserId { get; set; }
}