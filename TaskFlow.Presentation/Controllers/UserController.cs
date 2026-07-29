using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Dtos.UserDtos;
using TaskFlow.Infrastructure.UseCase;

namespace TaskFlow.Presentation.Controllers;
[Authorize]
[ApiController]
[Route("[controller]")]
public class UsersController(UsersUseCase usersUseCase) : ControllerBase
{
    private readonly UsersUseCase _usersUseCase = usersUseCase;

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserById([FromRoute] Guid userId)
    {
        var requesterIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var requesterId = Guid.Parse(requesterIdString!);

        var dto = new GetUserByIdDto(requesterId, userId);

        var result = await _usersUseCase.GetUserByIdAsync(dto);

        return Ok(new
        {
            message = "User retrieved successfully.",
            data = result
        });
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAllUsers()
    {
        var requesterIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var requesterId = Guid.Parse(requesterIdString!);

        var dto = new GetUsersDto(requesterId);

        var result = await _usersUseCase.GetUsersAsync(dto);

        return Ok(new
        {
            message = "Users retrieved successfully.",
            data = result
        });
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUser([FromRoute] Guid userId)
    {
        var requesterIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var requesterId = Guid.Parse(requesterIdString!);

        var dto = new DeleteUserDto(requesterId, userId);

        await _usersUseCase.DeleteUserAsync(dto);

        return Ok(new
        {
            message = "User deleted successfully."
        });
    }
}
