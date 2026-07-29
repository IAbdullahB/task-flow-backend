using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Dtos.TaskItemDtos;
using TaskFlow.Infrastructure.UseCase;
using TaskFlow.Presentation.ViewModels.TaskItemVMs;

namespace TaskFlow.Presentation.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class TaskController(
    TaskItemsUseCase taskUseCase
    ) : ControllerBase
{
    private readonly TaskItemsUseCase _taskUseCase = taskUseCase;

    [HttpPost("create")]
    public async Task<IActionResult> CreateTask([FromBody] CreateNewTaskVM vm)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userId = Guid.Parse(userIdString!);

        var dto = new CreateNewTaskDto(
            CreatorUserId: userId,
            AssignedUserId: vm.AssignedUserId,
            Title: vm.Title,
            Description: vm.Description,
            DueDate: vm.DueDate);

        var result = await _taskUseCase.CreateNewTaskAsync(dto);

        return Created("", new
        {
            message = "Task created successfully.",
            data = result
        });
    }

    [HttpGet("{taskId}")]
    public async Task<IActionResult> GetTaskById([FromRoute] Guid taskId)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userId = Guid.Parse(userIdString!);

        var dto = new GetTaskByIdDto(userId, taskId);

        var result = await _taskUseCase.GetTaskByIdAsync(dto);

        return Ok(new
        {
            message = "Task retrieved successfully.",
            data = result
        });
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAllTasks()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userId = Guid.Parse(userIdString!);

        var dto = new GetAllTasksDto(userId);

        var result = await _taskUseCase.GetAllTasksAsync(dto);

        return Ok(new
        {
            message = "Tasks retrieved successfully.",
            data = result
        });
    }

    [HttpDelete("{taskId}")]
    public async Task<IActionResult> DeleteTask([FromRoute] Guid taskId)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userId = Guid.Parse(userIdString!);

        var dto = new DeleteTaskDto(userId, taskId);

        await _taskUseCase.DeleteTaskAsync(dto);

        return Ok(new
        {
            message = "Task deleted successfully."
        });
    }

    [HttpPatch("reassign")]
    public async Task<IActionResult> ReassignTask([FromBody] ReassignTaskVM vm)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userId = Guid.Parse(userIdString!);

        var dto = new ReassignTaskDto(userId, vm.TaskId, vm.NewAssignedUserId);

        await _taskUseCase.ReassignTaskAsync(dto);

        return Ok(new
        {
            message = "Task reassigned successfully."
        });
    }
}
