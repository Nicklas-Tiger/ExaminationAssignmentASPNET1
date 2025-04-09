using Business.Mapping;
using Business.Models;
using Data.Entities;
using Data.Repositories;
using Domain.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.Identity;


namespace Business.Services;

public interface IUserService
{
    Task<UserResult> AddUserToRoleAsync(string userId, string roleName);
    Task<UserResult> GetUsersAsync();
    Task<string> GetDisplayName(string userId);
    Task<UserResult<User>> GetUserByIdAsync(string id);
    Task<UserResult> UserExistsByEmailAsync(string email);

}

public class UserService(IUserRepository userRepository, UserManager<UserEntity> userManager, RoleManager<IdentityRole> roleManager) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;


    public async Task<UserResult> GetUsersAsync()
    {
        var result = await _userRepository.GetAllAsync();
        return result.MapTo<UserResult>();

    }

    public async Task<UserResult> AddUserToRoleAsync(string userId, string roleName)
    {
        if (!await _roleManager.RoleExistsAsync(roleName))
            return new UserResult { Succeeded = false, Error = "Role doesn't exist." };

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return new UserResult { Succeeded = false, StatusCode = 404, Error = "User doesn't exist." };

        var result = await _userManager.AddToRoleAsync(user, roleName);
        return result.Succeeded
            ? new UserResult { Succeeded = true, StatusCode = 200 }
            : new UserResult { Succeeded = false, StatusCode = 500, Error = "Unable to add user to role." };
    }
    public async Task<UserResult> UserExistsByEmailAsync(string email)
    {
        var existsResult = await _userRepository.ExistsAsync(x => x.Email == email);
        if (existsResult.Succeeded)
            return new UserResult { Succeeded = true, StatusCode = 200, Error = "A user with the specified email address exists." };

        return new UserResult { Succeeded = false, StatusCode = 404, Error = "User was not found." };
    }

    public async Task<UserResult<User>> GetUserByIdAsync(string id)
    {
        var repositoryResult = await _userRepository.GetAsync(x => x.Id == id);

        var entity = repositoryResult.Result;
        if (entity == null)
            return new UserResult<User> { Succeeded = false, StatusCode = 404, Error = $"User with id '{id}' was not found." };

        var user = entity.MapTo<User>();
        return new UserResult<User> { Succeeded = true, StatusCode = 200, Result = user };

    }


    public async Task<string> GetDisplayName(string userId)
    {
        if (string.IsNullOrEmpty(userId))
            return "";
        var user = await _userManager.FindByIdAsync(userId);
        return user == null ? "" : $"{user.FirstName} {user.LastName}";
    }

}
