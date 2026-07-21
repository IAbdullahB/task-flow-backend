using TaskFlow.Application.Dtos.TaskItemDtos;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.ReposInterfaces;

namespace TaskFlow.Infrastructure.UseCase;
public class TaskItemsUseCase(
    ITaskItemRepository taskItemRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork
    )
{
    private readonly ITaskItemRepository _taskItemRepository = taskItemRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Guid> CreateNewTaskAsync(CreateNewTaskDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title)) throw new ArgumentException("Title must be provided");   

        var creatorUser = await _userRepository.GetByIdAsync(dto.CreatorUserId);
        if (creatorUser == null) throw new Exception("Wrong user ID");
        
        Guid assignedUserId;
        if (creatorUser.Role == Domain.Enums.UserRole.Admin)
        {
            if (dto.AssignedUserId == null) throw new ArgumentException("AssignedUserId must be provided");

            var assignedUser = await _userRepository.GetByIdAsync(dto.AssignedUserId.Value);

            if (assignedUser == null) throw new Exception("Assigned user not found");
            
            assignedUserId = assignedUser.Id;
        }
        else assignedUserId = creatorUser.Id;
        

        var taskItem = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = dto.Title.Trim(),
            Description = dto.Description,
            DueDate = dto.DueDate,
            AssignedUserId = assignedUserId,
            IsDone = false
        };

        await _taskItemRepository.InsertAsync(taskItem);
        await _unitOfWork.SaveAsync();

        return taskItem.Id;
    }

    public async Task<TaskItem> GetTaskByIdAsync(GetTaskByIdDto dto)
    {
        var user = await _userRepository.GetByIdAsync(dto.RequesterUserId);
        if (user == null) throw new Exception("Wrong user ID");

        var task = await _taskItemRepository.GetByIdAsync(dto.TaskId);
        if (task == null) throw new Exception("Task not found");

        if (user.Role != Domain.Enums.UserRole.Admin &&
            task.AssignedUserId != dto.RequesterUserId)
        {
            throw new Exception("Access denied");
        }

        return task;
    }

    public async Task<IEnumerable<TaskItem>> GetAllTasksAsync(GetAllTasksDto dto)
    {
        var user = await _userRepository.GetByIdAsync(dto.RequesterId);
        if (user == null) throw new Exception("Wrong user ID");

        var allTasks = await _taskItemRepository.GetAllAsync();

        if (user.Role == Domain.Enums.UserRole.Admin) return allTasks;
        else return allTasks.Where(task => task.AssignedUserId == user.Id);
    }

    public async Task DeleteTaskAsync(DeleteTaskDto dto)
    {
        var user = await _userRepository.GetByIdAsync(dto.RequesterUserId);
        if (user == null) throw new Exception("Wrong user ID");

        if (user.Role != Domain.Enums.UserRole.Admin) throw new Exception("Access denied");

        var task = await _taskItemRepository.GetByIdAsync(dto.TaskId);
        if (task == null) throw new Exception("Task not found");

        await _taskItemRepository.DeleteAsync(task);
        await _unitOfWork.SaveAsync();
    }

    public async Task ReassignTaskAsync(ReassignTaskDto dto)
    {
        var user = await _userRepository.GetByIdAsync(dto.RequesterUserId);
        if (user == null) throw new Exception("Wrong user ID");
        if (user.Role != Domain.Enums.UserRole.Admin) throw new Exception("Access denied");

        var task = await _taskItemRepository.GetByIdAsync(dto.TaskId);
        if (task == null) throw new Exception("Task not found");

        var newAssignedUser = await _userRepository.GetByIdAsync(dto.NewAssignedUserId);
        if (newAssignedUser == null) throw new Exception("Assigned user not found");

        task.AssignedUserId = dto.NewAssignedUserId;

        await _unitOfWork.SaveAsync();
    }
}