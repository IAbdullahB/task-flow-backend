using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.RepoInterfaces;

namespace TaskFlow.Infrastructure.Repositories;
public class UserRepository(ApplicationDbContext context) : Repository<User>(context), IUserRepository
{
    private readonly ApplicationDbContext _context = context;

    public UserRole? GetRoleById(Guid id)
    {
        var user = _context.Set<User>().Find(id);
        return user?.Role;
    }

    public void UpdateRole(User user, UserRole newRole)
    {
        user.Role = newRole;
        _context.Set<User>().Update(user);
    }
}
