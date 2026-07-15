
namespace TaskFlow.Domain.ReposInterfaces;
public interface IUnitOfWork : IDisposable
{
    void Save();
}
