using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Domain.ReposInterfaces;
public interface IUserRepository 
{
    Task InsertAsync(User user);
    Task DeleteAsync(User user);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(Guid id);
    Task<UserRole?> GetRoleByIdAsync(Guid id);
}
