
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using TaskFlow.Application.Dtos.RequestDtos.AuthenticationDtos;
using TaskFlow.Application.Exceptions;
using TaskFlow.Application.ServicesInterfaces;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.ReposInterfaces;
using static System.Net.WebRequestMethods;

namespace TaskFlow.Infrastructure.UseCase;
public class AuthenticationUseCase(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IJwtService jwtService,
    ICacheService cacheService,
    ILogger<AuthenticationUseCase> logger
    )
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IJwtService _jwtService = jwtService;
    private readonly ICacheService _cacheService = cacheService;
    private readonly ILogger _logger = logger;

    public async Task Register(RegisterUserDto dto, bool staySignedIn = false)
    {
        if (await _userRepository.IsEmailExistAsync(dto.Email)) throw new ConflictException("Email already exists");

        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = HashPassword(dto.Password, salt);

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

        string otp = Random.Shared.Next(100000, 1000000).ToString();
        string cacheKey = $"otp:verify:{user.Email}";

        await _cacheService.SetAsync(cacheKey, otp, TimeSpan.FromMinutes(10));

        _logger.LogInformation("Verification OTP for user {Email}: {Otp}", user.Email, otp);
    }

    public async Task<string> Login(LoginUserDto dto, bool staySignedIn = false)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);
        if (user == null) throw new UnauthorizedException("Invalid email or password");
        if(!user.IsVerified) throw new UnauthorizedException("Email is not verified");

        var salt = Convert.FromBase64String(user.SaltPassword);
        var enteredHash = HashPassword(dto.Password, salt);

        var actualHash = Convert.FromBase64String(user.HashPassword);

        if (!CryptographicOperations.FixedTimeEquals(enteredHash, actualHash))
            throw new UnauthorizedException("Invalid email or password");

        return _jwtService.GenerateToken(user, staySignedIn);
    }

    public async Task ChangePassword(Guid requesterId, ChangePasswordDto dto)
    {
        if (requesterId == Guid.Empty) throw new ArgumentException("Invalid requester ID");
        
        var user = await _userRepository.GetByIdAsync(requesterId) ?? throw new NotFoundException("User not found");

        var salt = Convert.FromBase64String(user.SaltPassword);
        var enteredHash = HashPassword(dto.CurrentPassword, salt);

        var actualHash = Convert.FromBase64String(user.HashPassword);
        if (!CryptographicOperations.FixedTimeEquals(enteredHash, actualHash))
            throw new UnauthorizedException("Wrong Password");

        var newSalt = RandomNumberGenerator.GetBytes(16);
        var newHash = HashPassword(dto.NewPassword, newSalt);

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

        var cacheKey = $"otp:reset:{dto.Email.Trim().ToLower()}";

        await _cacheService.SetAsync(cacheKey, otp, TimeSpan.FromMinutes(10));

        _logger.LogInformation("Password reset OTP for user {Email}: {Otp}", dto.Email, otp);
    }

    public async Task ResetPassword(ResetPasswordDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email)) throw new ArgumentException("Email is required");
        if (string.IsNullOrWhiteSpace(dto.Otp)) throw new ArgumentException("OTP is required");
        if (string.IsNullOrWhiteSpace(dto.NewPassword)) throw new ArgumentException("New password is required");

        var user = await _userRepository.GetByEmailAsync(dto.Email);
        if (user == null) throw new NotFoundException("User not found");

        var cacheKey = $"otp:reset:{dto.Email.Trim().ToLower()}";

        var cachedOtp = await _cacheService.GetAsync<string>(cacheKey);

        if (string.IsNullOrEmpty(cachedOtp) || cachedOtp != dto.Otp)
            throw new ValidationException("The OTP is invalid or has expired.");
        

        await _cacheService.RemoveAsync(cacheKey);


        var newSalt = RandomNumberGenerator.GetBytes(16);
        var newHash = HashPassword(dto.NewPassword, newSalt);

        user.SaltPassword = Convert.ToBase64String(newSalt);
        user.HashPassword = Convert.ToBase64String(newHash);

        await _unitOfWork.SaveAsync();
    }

    private static byte[] HashPassword(string password, byte[] salt)
    {
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            100_000,
            HashAlgorithmName.SHA256,
            32);
        return hash;
    }

    public async Task VerifyEmail(VerifyEmailDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);
        if (user == null) throw new NotFoundException("User not found");
        if (user.IsVerified) throw new ValidationException("Email is already verified");

        string cacheKey = $"otp:verify:{user.Email}";

        string? cachedOtp = await _cacheService.GetAsync<string>(cacheKey);
        if (string.IsNullOrEmpty(cachedOtp)) throw new ValidationException("The OTP is invalid or has expired.");

        if (cachedOtp != dto.Otp) throw new ValidationException("The OTP is invalid or has expired.");

        user.IsVerified = true;
        await _unitOfWork.SaveAsync();

        await _cacheService.RemoveAsync(cacheKey);
    }

    public async Task ResendVerificationOtp(ResendOtpDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);
        if (user == null) throw new NotFoundException("User not found");
        if (user.IsVerified) throw new ValidationException("Account is already verified.");

        var salt = Convert.FromBase64String(user.SaltPassword);
        var enteredHash = HashPassword(dto.Password, salt);

        var actualHash = Convert.FromBase64String(user.HashPassword);

        if (!CryptographicOperations.FixedTimeEquals(enteredHash, actualHash))
            throw new UnauthorizedException("Invalid email or password");

        string newOtp = Random.Shared.Next(100000, 999999).ToString();
        string cacheKey = $"otp:verify:{user.Email}";

        await _cacheService.SetAsync(cacheKey, newOtp, TimeSpan.FromMinutes(10));

        _logger.LogInformation("Password reset OTP for user {Email}: {Otp}", dto.Email, newOtp);
    }

    public async Task Logout(LogoutUserDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Token)) throw new ArgumentException("Token is required");

        var cleanToken = dto.Token.Trim();
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(cleanToken);

        var remainingTime = jwtToken.ValidTo - DateTime.UtcNow;

        if (remainingTime <= TimeSpan.Zero) return;

        var cacheExpiration = remainingTime.Add(TimeSpan.FromMinutes(1));

        var cacheKey = $"jwt:blacklist:{cleanToken}";
        await _cacheService.SetAsync(cacheKey, true, cacheExpiration);
    }

}
