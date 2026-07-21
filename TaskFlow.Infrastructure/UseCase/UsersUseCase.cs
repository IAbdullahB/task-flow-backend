
using TaskFlow.Application.Dtos.UserDtos;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.ReposInterfaces;

namespace TaskFlow.Infrastructure.UseCase;
public class UsersUseCase(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork
    )
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<User> GetUserByIdAsync(GetUserByIdDto dto)
    {
        var user = await _userRepository.GetByIdAsync(dto.RequesterId);
        if (user == null) throw new Exception("Wrong user ID");

        if(user.Role != UserRole.Admin) throw new Exception("Access denied");

        return user;
    }

    public async Task<IEnumerable<User>> GetUsersAsync(GetUsersDto dto)
    {
        var user = await _userRepository.GetByIdAsync(dto.RequesterId);
        if (user == null) throw new Exception("Wrong user ID");

        if (user.Role != UserRole.Admin) throw new Exception("Access denied");
        var allUsers = await _userRepository.GetAllAsync();

        return allUsers;
    }

    public async Task DeleteUserAsync(DeleteUserDto dto)
    {
        var user = await _userRepository.GetByIdAsync(dto.RequesterId);
        if (user == null) throw new Exception("Wrong user ID");

        if (user.Role != UserRole.Admin) throw new Exception("Access denied");

        var deletedUser = await _userRepository.GetByIdAsync(dto.UserId);
        if (deletedUser == null) throw new Exception("User not found");

        await _userRepository.DeleteAsync(deletedUser);
        await _unitOfWork.SaveAsync();
    }
}
