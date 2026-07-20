using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.ReposInterfaces;

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
        return await _context.Users.Where(u => u.Id == id).Select(u => (UserRole?)u.Role).FirstOrDefaultAsync();
    }

    public Task DeleteAsync(User user)
    {
        _context.Users.Remove(user);
        return Task.CompletedTask;
    }

}
