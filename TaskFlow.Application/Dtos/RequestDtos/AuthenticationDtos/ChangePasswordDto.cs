namespace TaskFlow.Application.Dtos.RequestDtos.AuthenticationDtos;

public record ChangePasswordDto(string CurrentPassword, string NewPassword);