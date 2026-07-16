using TaskFlow.Domain.Entities;
using TaskFlow.Domain.ReposInterfaces;

namespace TaskFlow.Domain.RepoInterfaces;
public interface ITaskItemRepository 
{
    Task InsertAsync(TaskItem taskItem);
    Task DeleteAsync(TaskItem taskItem);
    Task<IEnumerable<TaskItem>> GetAllAsync();
    Task<TaskItem?> GetByIdAsync(Guid id);
    Task UpdateTitleAsync(TaskItem taskItem, string newTitle);
    Task UpdateDescriptionAsync(TaskItem taskItem, string? newDescription);
    Task UpdateStatusAsync(TaskItem taskItem, bool isDone);
}
