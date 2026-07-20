
namespace TaskFlow.Domain.ReposInterfaces;
public interface IUnitOfWork : IDisposable
{
    Task SaveAsync();
}
