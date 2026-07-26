namespace TaskFlow.Application.Dtos.AuthenticationDtos;

public record ResetPasswordDto(string Email, string Otp, string NewPassword);    
