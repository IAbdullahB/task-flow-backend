using TaskFlow.Domain.ReposInterfaces;

namespace TaskFlow.Infrastructure.Repositories;
public class Repository<T>(ApplicationDbContext context) : IRepository<T> where T : class
{
    private readonly ApplicationDbContext _context = context;

    public void Insert(T entity)
    {
        _context.Set<T>().Add(entity);
    }

    public void Update(T entity)
    {
        _context.Set<T>().Update(entity);
    }

    public void Delete(T entity)
    {
        _context.Set<T>().Remove(entity);
    }

    public IEnumerable<T> GetAll()
    {
        return _context.Set<T>().ToList();
    }

    public T? GetById(Guid id)
    {
        return _context.Set<T>().Find(id);
    }
}
