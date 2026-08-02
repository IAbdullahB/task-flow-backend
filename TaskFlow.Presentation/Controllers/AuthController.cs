using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Dtos.RequestDtos.AuthenticationDtos;
using TaskFlow.Infrastructure.UseCase;
using TaskFlow.Presentation.ViewModels.AuthenticationVMs;

namespace TaskFlow.Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(
    AuthenticationUseCase authUseCase
    ) : ControllerBase
{
    private readonly AuthenticationUseCase _authUseCase = authUseCase;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserVM vm)
    {
        var dto = new RegisterUserDto(vm.UserName, vm.Email, vm.Password);

        await _authUseCase.Register(dto);

        return Created("", new
        {
            message = "User registered successfully," +
            " please check your email for the verification OTP",
        });
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailVM vm)
    {
        var dto = new VerifyEmailDto(vm.Email, vm.Otp);
        await _authUseCase.VerifyEmail(dto);
        return Ok(new
        {
            message = "Email verified successfully :).",
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserVM vm)
    {
        var dto = new LoginUserDto(vm.Email, vm.Password);

        var result = await _authUseCase.Login(dto);

        return Ok(new
        {
            message = "User logged in successfully :D.",
            token = result
        });
    }

    [Authorize] 
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordVM vm)
    {
        var dto = new ChangePasswordDto(vm.CurrentPassword, vm.NewPassword);

        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userId = Guid.Parse(userIdString!);

        await _authUseCase.ChangePassword(userId, dto);

        return Ok(new
        {
            message = "Password changed successfully :D.",
        });
    }

    [HttpPost("request-password-reset")]
    public async Task<IActionResult> RequestPasswordReset([FromBody] RequestPasswordResetVM vm)
    {
        var dto = new RequestPasswordResetDto(vm.Email);

        await _authUseCase.RequestPasswordReset(dto);

        return Ok(new
        {
            message = "Password reset requested successfully," +
            " please check your email for the reset OTP",
        });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordVM vm)
    {
        var dto = new ResetPasswordDto(vm.Email, vm.Otp, vm.NewPassword);

        await _authUseCase.ResetPassword(dto);

        return Ok(new
        { 
            message = "Password has been successfully reset." 
        });
    }
    
    [HttpPost("resend-verification-otp")]
    public async Task<IActionResult> ResendVerificationOtp([FromBody] ResendOtpVM vm)
    {
        var dto = new ResendOtpDto(vm.Email, vm.Password);
        await _authUseCase.ResendVerificationOtp(dto);
        return Ok(new
        {
            message = "Verification OTP resent successfully.",
        });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        var dto = new LogoutUserDto(token);
           
        await _authUseCase.Logout(dto);
        return Ok(new
        {
            message = "User logged out successfully :(.",
        });
    }
}