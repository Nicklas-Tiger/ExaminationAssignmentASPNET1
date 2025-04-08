using Business.Models;
using Data.Entities;
using Domain.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Business.Services;

public interface IAuthService
{
    Task<AuthResult> SignInAsync(SignInFormData formData);
    Task<AuthResult> SignOutAsync();
    Task<AuthResult> SignUpAsync(SignUpFormData formData);
}

public class AuthService(IUserService userService, UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager) : IAuthService
{
    private readonly IUserService _userService = userService;
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly SignInManager<UserEntity> _signInManager = signInManager;


    public async Task<AuthResult> SignInAsync(SignInFormData formData)
    {
        if (formData == null)
            return new AuthResult() { Succeeded = false, StatusCode = 400, Error = "Not all required fields are supplied." };

        var result = await _signInManager.PasswordSignInAsync(formData.Email, formData.Password, formData.isPersistent, lockoutOnFailure: false);
        return result.Succeeded
            ? new AuthResult { Succeeded = true, StatusCode = 201 }
            : new AuthResult { Succeeded = false, StatusCode = 401, Error = "Invalid email or password." };
    }

    public async Task<AuthResult> SignUpAsync(SignUpFormData formData)
    {
        if (formData == null)
            return new AuthResult { Succeeded = false, StatusCode = 400, Error = "form data can't be null." };

        var userResult = await _userService.UserExistsByEmailAsync(formData.Email);
        if (userResult.Succeeded)
            return new AuthResult { Succeeded = false, StatusCode = 409, Error = userResult.Error };

        try
        {
            var userEntity = formData.MapTo<UserEntity>();
            userEntity.UserName = userEntity.Email;

            var identityResult = await _userManager.CreateAsync(userEntity, formData.Password);
            if (identityResult.Succeeded)
            {
                if (formData.RoleName != null)
                {
                    var result = await _userService.AddUserToRoleAsync(userEntity.Id, formData.RoleName);
                }
                await _signInManager.SignInAsync(userEntity, isPersistent: false);
                return new AuthResult { Succeeded = true, StatusCode = 201, SuccessMessage = $"User was created successfully." };
            }

            throw new Exception("Unable to sign up user");

        }
        catch (Exception ex)
        {
            return new AuthResult { Succeeded = false, StatusCode = 500, Error = ex.Message };
        }
    }

    public async Task<AuthResult> SignOutAsync()
    {
        await _signInManager.SignOutAsync();
        return new AuthResult { Succeeded = true, StatusCode = 200 };
    }
}