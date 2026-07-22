
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using TaskFlow.Application.Dtos.AuthenticationDtos;
using TaskFlow.Application.Dtos.UserDtos;
using TaskFlow.Application.ServicesInterfaces;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.ReposInterfaces;

namespace TaskFlow.Infrastructure.UseCase;
public class AuthenticationUseCase(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IJwtService jwtService,
    ILogger logger
    )
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IJwtService _jwtService = jwtService;
    private readonly ILogger _logger = logger;

    public async Task<string> Register(RegisterUserDto dto, bool staySignedIn = false)
    {
        if (string.IsNullOrWhiteSpace(dto.UserName)) throw new ArgumentException("User name is required");
        if (string.IsNullOrWhiteSpace(dto.Email)) throw new ArgumentException("Email is required");
        if (string.IsNullOrWhiteSpace(dto.Password)) throw new ArgumentException("Password is required");

        if (await _userRepository.IsEmailExistAsync(dto.Email)) throw new ConflictException("Email already exists");

        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            dto.Password,
            salt,
            100_000,
            HashAlgorithmName.SHA256,
            32);

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = dto.UserName,
            Email = dto.Email,
            HashPassword = Convert.ToBase64String(hash),
            SaltPassword = Convert.ToBase64String(salt),
            Role = UserRole.Member
        };

        await _userRepository.InsertAsync(user);
        await _unitOfWork.SaveAsync();

        return _jwtService.GenerateToken(user, staySignedIn);

    }

    public async Task<string> Login(LoginUserDto dto, bool staySignedIn = false)
    {
        if (string.IsNullOrWhiteSpace(dto.Email)) throw new ArgumentException("Email is required");
        if (string.IsNullOrWhiteSpace(dto.Password)) throw new ArgumentException("Password is required");

        var user = await _userRepository.GetByEmailAsync(dto.Email);
        if (user == null ||
            !await _userRepository.IsEmailExistAsync(dto.Email)) throw new UnauthorizedException("Invalid email or password");

        var salt = Convert.FromBase64String(user.SaltPassword);
        var enteredHash = Rfc2898DeriveBytes.Pbkdf2(
            dto.Password,
            salt,
            100_000,
            HashAlgorithmName.SHA256,
            32);

        var actualHash = Convert.FromBase64String(user.HashPassword);

        if (!CryptographicOperations.FixedTimeEquals(enteredHash, actualHash))
            throw new Exception("Invalid email or password");

        return _jwtService.GenerateToken(user, staySignedIn);
    }

    public async Task ChangePassword(Guid requesterId, ChangePasswordDto dto)
    {
        if (requesterId == Guid.Empty) throw new ArgumentException("Invalid requester ID");
        if (string.IsNullOrWhiteSpace(dto.CurrentPassword)) throw new ArgumentException("Old password is required");
        if (string.IsNullOrWhiteSpace(dto.NewPassword)) throw new ArgumentException("New password is required");

        var user = await _userRepository.GetByIdAsync(requesterId) ?? throw new NotFoundException("User not found");

        var salt = Convert.FromBase64String(user.SaltPassword);
        var enteredHash = Rfc2898DeriveBytes.Pbkdf2(
            dto.CurrentPassword,
            salt,
            100_000,
            HashAlgorithmName.SHA256,
            32);

        var actualHash = Convert.FromBase64String(user.HashPassword);
        if (!CryptographicOperations.FixedTimeEquals(enteredHash, actualHash))
            throw new UnauthorizedException("Wrong Password");

        var newSalt = RandomNumberGenerator.GetBytes(16);
        var newHash = Rfc2898DeriveBytes.Pbkdf2(
            dto.NewPassword,
            newSalt,
            100_000,
            HashAlgorithmName.SHA256,
            32);

        user.SaltPassword = Convert.ToBase64String(newSalt);
        user.HashPassword = Convert.ToBase64String(newHash);

        await _unitOfWork.SaveAsync();

    }

    public async Task RequestPasswordReset(RequestPasswordResetDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email)) throw new ArgumentException("Email is required");

        var user = await _userRepository.GetByEmailAsync(dto.Email);
        if (user == null) throw new NotFoundException("User not found");

        var otp = RandomNumberGenerator.GetInt32(100000, 1000000).ToString("D6");

        user.ResetPasswordOtp = otp;
        user.ResetPasswordOtpExpiresAt = DateTimeOffset.UtcNow.AddMinutes(10);
        await _unitOfWork.SaveAsync();

        _logger.LogInformation("Password reset OTP for user {Email}: {Otp}", dto.Email, otp);
    }

    public async Task ForgotPassword(ResetPasswordDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email)) throw new ArgumentException("Email is required");
        if (string.IsNullOrWhiteSpace(dto.Otp)) throw new ArgumentException("OTP is required");
        if (string.IsNullOrWhiteSpace(dto.NewPassword)) throw new ArgumentException("New password is required");

        var user = await _userRepository.GetByEmailAsync(dto.Email);
        if (user == null) throw new NotFoundException("User not found");

        if (string.IsNullOrWhiteSpace(user.ResetPasswordOtp) || user.ResetPasswordOtpExpiresAt < DateTimeOffset.UtcNow)
            throw new UnauthorizedException("OTP expired");
        

        if (!string.Equals(user.ResetPasswordOtp, dto.Otp, StringComparison.Ordinal))
            throw new UnauthorizedException("Invalid OTP");
        

        var newSalt = RandomNumberGenerator.GetBytes(16);
        var newHash = Rfc2898DeriveBytes.Pbkdf2(
            dto.NewPassword,
            newSalt,
            100_000,
            HashAlgorithmName.SHA256,
            32);

        user.SaltPassword = Convert.ToBase64String(newSalt);
        user.HashPassword = Convert.ToBase64String(newHash);
        user.ResetPasswordOtp = null;
        user.ResetPasswordOtpExpiresAt = null;

        await _unitOfWork.SaveAsync();
    }

}
