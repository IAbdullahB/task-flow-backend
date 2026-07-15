using TaskFlow.Domain.Entities;
using TaskFlow.Domain.ReposInterfaces;

namespace TaskFlow.Domain.RepoInterfaces;
public interface ITaskItemRepository : IRepository<TaskItem>
{
    void UpdateTitle(TaskItem taskItem, string newTitle);
    void UpdateDescription(TaskItem taskItem, string? newDescription);
    void UpdateStatus(TaskItem taskItem, bool isDone);
}
