using TaskFlow.Application.Dtos.RequestDtos.AuthenticationDtos;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.ServicesInterfaces;
public interface IJwtService
{
    string GenerateToken(User user, bool staySignedIn = false);
    Task<T> ValidateToken<T>(string token);
}
