using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;
using TaskFlow.Application.Exceptions;
using TaskFlow.Domain.ReposInterfaces;
using TaskFlow.Application.Dtos.RequestDtos.UserDtos;

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
        if (user == null) throw new NotFoundException("Wrong user ID");

        if (user.Role != UserRole.Admin) throw new AccessDeniedException("Access denied");

        var requestedUser = await _userRepository.GetByIdAsync(dto.UserId);
        if (requestedUser == null) throw new NotFoundException("User not found");

        return requestedUser;
    }

    public async Task<IEnumerable<User>> GetUsersAsync(GetUsersDto dto)
    {
        var user = await _userRepository.GetByIdAsync(dto.RequesterId);
        if (user == null) throw new NotFoundException("Wrong user ID");

        if (user.Role != UserRole.Admin) throw new AccessDeniedException("Access denied");

        var allUsers = await _userRepository.GetAllAsync();

        return allUsers;
    }

    public async Task DeleteUserAsync(DeleteUserDto dto)
    {
        var user = await _userRepository.GetByIdAsync(dto.RequesterId);
        if (user == null) throw new NotFoundException("Wrong user ID");

        if (user.Role != UserRole.Admin) throw new AccessDeniedException("Access denied");

        var deletedUser = await _userRepository.GetByIdAsync(dto.UserId);
        if (deletedUser == null) throw new NotFoundException("User not found");

        await _userRepository.DeleteAsync(deletedUser);
        await _unitOfWork.SaveAsync();
    }
}