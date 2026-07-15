using TaskFlow.Domain.ReposInterfaces;

namespace TaskFlow.Infrastructure.Repositories;
public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    private readonly ApplicationDbContext _context = context;

    public void Save()
    {
        _context.SaveChanges();
    }
    public void Dispose()
    {
        _context.Dispose();
    }
}

