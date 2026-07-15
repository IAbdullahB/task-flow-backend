using System;
using System.Collections.Generic;
using System.Linq;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.ReposInterfaces;

namespace TaskFlow.Domain.RepoInterfaces;
public interface IUserRepository : IRepository<User>
{
    UserRole? GetRoleById(Guid id);
    void UpdateRole(User user, UserRole newRole);
}
