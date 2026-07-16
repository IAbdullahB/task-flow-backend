using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.RepoInterfaces;

namespace TaskFlow.Infrastructure.Repositories;
public class UserRepository(ApplicationDbContext context) : IUserRepository
{
    private readonly ApplicationDbContext _context = context;
    public async Task InsertAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<UserRole?> GetRoleByIdAsync(Guid id)
    {
        return await _context.Users
            .Where(u => u.Id == id)
            .Select(u => u.Role)
            .FirstOrDefaultAsync();
    }

    public async Task UpdateRoleAsync(User user, UserRole newRole)
    {
        await _context.Users.Where(u => u.Id == user.Id)
            .ExecuteUpdateAsync(u => u.SetProperty(u => u.Role, newRole));
    }

    public Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        return Task.CompletedTask;

    }

    public Task DeleteAsync(User user)
    {
        _context.Users.Remove(user);
        return Task.CompletedTask;
    }

}
