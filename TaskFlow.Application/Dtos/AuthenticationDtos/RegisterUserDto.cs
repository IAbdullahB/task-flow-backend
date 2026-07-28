namespace TaskFlow.Application.Dtos.AuthenticationDtos;

public record RegisterUserDto(
    string UserName,
    string Email,
    string Password);