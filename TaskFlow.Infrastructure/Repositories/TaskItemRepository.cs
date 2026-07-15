using TaskFlow.Domain.Entities;
using TaskFlow.Domain.RepoInterfaces;

namespace TaskFlow.Infrastructure.Repositories;
public class TaskItemRepository(ApplicationDbContext context) : Repository<TaskItem>(context), ITaskItemRepository 
{
    private readonly ApplicationDbContext _context = context;

    public void UpdateDescription(TaskItem taskItem, string? newDescription)
    {
        taskItem.Description = newDescription;
        _context.Set<TaskItem>().Update(taskItem);
    }

    public void UpdateStatus(TaskItem taskItem, bool isDone)
    {
        taskItem.IsDone = isDone;
        _context.Set<TaskItem>().Update(taskItem);
    }

    public void UpdateTitle(TaskItem taskItem, string newTitle)
    {
        taskItem.Title = newTitle;
        _context.Set<TaskItem>().Update(taskItem);
    }
}
