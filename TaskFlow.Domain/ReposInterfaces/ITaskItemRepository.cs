using TaskFlow.Domain.Entities;

namespace TaskFlow.Domain.ReposInterfaces;
public interface ITaskItemRepository 
{
    Task InsertAsync(TaskItem taskItem);
    Task DeleteAsync(TaskItem taskItem);
    Task<IEnumerable<TaskItem>> GetAllAsync();
    Task<TaskItem?> GetByIdAsync(Guid id);
}
