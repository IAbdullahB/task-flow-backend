using TaskFlow.Application.Dtos.RequestDtos.TaskItemDtos;
using TaskFlow.Application.Dtos.ResponseDtos.TaskItemDtos;
using TaskFlow.Application.Exceptions;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;
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
        var creatorUser = await _userRepository.GetByIdAsync(dto.CreatorUserId);
        if (creatorUser == null) throw new NotFoundException("Wrong user ID");

        if (creatorUser.CanAssignTasksToOthers() && dto.AssignedUserId == null)
         throw new ArgumentException("Assigned User ID is required when you have permission to assign tasks.");
        
        var assignedUserId = creatorUser.CanAssignTasksToOthers()
        ? await ResolveAssignedUserId(dto.AssignedUserId!.Value)
        : creatorUser.Id;

        var taskItem = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = dto.Title.Trim(),
            Description = dto.Description?.Trim(),
            DueDate = dto.DueDate,
            AssignedUserId = assignedUserId,
            IsDone = false
        };

        await _taskItemRepository.InsertAsync(taskItem);
        await _unitOfWork.SaveAsync();

        return taskItem.Id;
    }

    private async Task<Guid> ResolveAssignedUserId(Guid assignedUserId)
    {
        var assignedUser = await _userRepository.GetByIdAsync(assignedUserId);
        if (assignedUser == null) throw new NotFoundException("Assigned user not found");

        return assignedUser.Id;
    }

    public async Task<TaskResponseDto> GetTaskByIdAsync(GetTaskByIdDto dto)
    {
        var user = await _userRepository.GetByIdAsync(dto.RequesterUserId);
        if (user == null) throw new NotFoundException("Wrong user ID");

        var task = await _taskItemRepository.GetByIdAsync(dto.TaskId);
        if (task == null) throw new NotFoundException("Task not found");

        if (user.Role != UserRole.Admin &&
            task.AssignedUserId != dto.RequesterUserId)
        {
            throw new AccessDeniedException("Access denied");
        }

        return new TaskResponseDto(
            task.Id,
            task.Title,
            task.Description ?? string.Empty,
            task.IsDone,
            task.DueDate,
            task.AssignedUserId,
            task.AssignedUser?.UserName 
        );
    }

    public async Task<IEnumerable<TaskResponseDto>> GetAllTasksAsync(GetAllTasksDto dto)
    {
        var user = await _userRepository.GetByIdAsync(dto.RequesterId);
        if (user == null) throw new NotFoundException("Wrong user ID");

        var allTasks = await _taskItemRepository.GetAllAsync();

        var filteredTasks = user.Role == UserRole.Admin
        ? allTasks
        : allTasks.Where(task => task.AssignedUserId == user.Id);

        return filteredTasks.Select(task => new TaskResponseDto(
            task.Id,
            task.Title,
            task.Description ?? string.Empty,
            task.IsDone,
            task.DueDate,
            task.AssignedUserId,
            task.AssignedUser?.UserName
        ));
    }

    public async Task DeleteTaskAsync(DeleteTaskDto dto)
    {
        var user = await _userRepository.GetByIdAsync(dto.RequesterUserId);
        if (user == null) throw new NotFoundException("Wrong user ID");

        if (user.Role != UserRole.Admin) throw new AccessDeniedException("Access denied");

        var task = await _taskItemRepository.GetByIdAsync(dto.TaskId);
        if (task == null) throw new NotFoundException("Task not found");

        await _taskItemRepository.DeleteAsync(task);
        await _unitOfWork.SaveAsync();
    }

    public async Task ReassignTaskAsync(ReassignTaskDto dto)
    {
        var user = await _userRepository.GetByIdAsync(dto.RequesterUserId);
        if (user == null) throw new NotFoundException("Wrong user ID");
        if (user.Role != UserRole.Admin) throw new AccessDeniedException("Access denied");

        var task = await _taskItemRepository.GetByIdAsync(dto.TaskId);
        if (task == null) throw new NotFoundException("Task not found");

        var newAssignedUser = await _userRepository.GetByIdAsync(dto.NewAssignedUserId);
        if (newAssignedUser == null) throw new NotFoundException("Assigned user not found");

        task.AssignedUserId = dto.NewAssignedUserId;

        await _unitOfWork.SaveAsync();
    }
}