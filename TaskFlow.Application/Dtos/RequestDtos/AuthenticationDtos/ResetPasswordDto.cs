namespace TaskFlow.Application.Dtos.RequestDtos.AuthenticationDtos;

public record ResetPasswordDto(string Email, string Otp, string NewPassword);    
