using System;
using System.Collections.Generic;
using System.Linq;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Domain.RepoInterfaces;
public interface IUserRepository 
{
    Task InsertAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(User user);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(Guid id);
    Task<UserRole?> GetRoleByIdAsync(Guid id);
    Task UpdateRoleAsync(User user, UserRole newRole);
}
