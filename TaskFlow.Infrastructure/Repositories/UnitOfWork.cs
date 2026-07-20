using TaskFlow.Domain.ReposInterfaces;

namespace TaskFlow.Infrastructure.Repositories;
public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    private readonly ApplicationDbContext _context = context;

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
    public void Dispose()
    {
        _context.Dispose();
    }
}

