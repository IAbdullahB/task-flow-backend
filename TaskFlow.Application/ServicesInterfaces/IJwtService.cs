using TaskFlow.Application.Dtos.AuthenticationDtos;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.ServicesInterfaces;
public interface IJwtService
{
    string GenerateToken(User user, bool staySignedIn = false); 
    T ValidateToken<T>(string token);
}
