namespace TaskFlow.Application.Dtos.RequestDtos.AuthenticationDtos;

public record RegisterUserDto(
    string UserName,
    string Email,
    string Password);