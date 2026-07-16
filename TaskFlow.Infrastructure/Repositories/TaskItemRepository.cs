using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.RepoInterfaces;

namespace TaskFlow.Infrastructure.Repositories;
public class TaskItemRepository(ApplicationDbContext context) : ITaskItemRepository 
{
    private readonly ApplicationDbContext _context = context;

    // we use async with the methods that we expect to be long-running operations and usually most of the time,
    // database operations are long-running operations, so we use async methods for database operations.
    // but in our case, all the database operations are expected to be fast.
    // but i wanna get used to the best practices xD
    public async Task InsertAsync(TaskItem taskItem)
    {
        await _context.Set<TaskItem>().AddAsync(taskItem);
    }


    public async Task<IEnumerable<TaskItem>> GetAllAsync()
    {
        return await _context.TaskItems.ToListAsync();
    }

    public async Task<TaskItem?> GetByIdAsync(Guid id)
    {
        return await _context.TaskItems.FindAsync(id);
    }

    // here we remove the async keyword becuase when we call .ExecuteDeleteAsync() 
    // it will completely bypass the change tracker and executes in the db immediately
    // and then when the change tracker reaches the SaveChangesAsync() method,
    // it will not find the entity in the database so it will throw an exception
    public Task DeleteAsync(TaskItem taskItem)
    {
        _context.TaskItems.Remove(taskItem);
        return Task.CompletedTask;
    }

    // here by keeping the async keyword and using .ExecuteUpdateAsync this will result in 
    // bypassing the change tracker and executing the update in the database immediately
    // so the update process will be not processed by the unit of work pattern 
    public async Task UpdateDescriptionAsync(TaskItem taskItem, string? newDescription)
    {
        await _context.TaskItems.Where(t => t.Id == taskItem.Id)
            .ExecuteUpdateAsync(t => t.SetProperty(t => t.Description, newDescription));
    }

    public async Task UpdateStatusAsync(TaskItem taskItem, bool isDone)
    {
        await _context.TaskItems.Where(t => t.Id == taskItem.Id)
            .ExecuteUpdateAsync(t => t.SetProperty(t => t.IsDone, isDone));
    }

    public async Task UpdateTitleAsync(TaskItem taskItem, string newTitle)
    {
        await _context.TaskItems.Where(t => t.Id == taskItem.Id)
            .ExecuteUpdateAsync(t => t.SetProperty(t => t.Title, newTitle));
    }
}
