namespace TaskFlow.Domain.ReposInterfaces;
public interface IRepository<EntityT> where EntityT : class
{
    void Insert(EntityT entity);
    void Update(EntityT entity);
    void Delete(EntityT entity);
    IEnumerable<EntityT> GetAll();
    EntityT? GetById(Guid id);
}
