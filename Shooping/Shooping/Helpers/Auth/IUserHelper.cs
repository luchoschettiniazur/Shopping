using Microsoft.AspNetCore.Identity;
using Shooping.Data.Identity;
using Shooping.Models;

namespace Shooping.Helpers.Auth;

public interface IUserHelper
{

    Task<User> GetUserAsync(string email);

    Task<IdentityResult> AddUserAsync(User user, string password);

	Task<User?> AddUserAsync(AddUserViewModel modelo);

	Task CheckRoleAsync(string roleName);

    Task AddUserToRoleAsync(User user, string roleName);

    Task<bool> IsUserInRoleAsync(User user, string roleName);



    Task<SignInResult> LoginAsync(LoginViewModel model);

    Task LogoutAsync();



    Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword);

    Task<IdentityResult> UpdateUserAsync(User user);

    Task<User> GetUserAsync(Guid userId);






}
