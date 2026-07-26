namespace TaskFlow.Application.Dtos.AuthenticationDtos;

public record ChangePasswordDto(string CurrentPassword, string NewPassword);