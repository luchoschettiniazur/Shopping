﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shooping.Data;
using Shooping.Data.Identity;
using Shooping.Models;

namespace Shooping.Helpers.Auth;

public class UserHelper : IUserHelper
{
    private readonly DataContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<User> _singnInManager;

    public UserHelper(DataContext context, UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager, SignInManager<User> singnInManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _singnInManager = singnInManager;
    }


    public async Task<IdentityResult> AddUserAsync(User user, string password)
    {
        return await _userManager.CreateAsync(user, password);

    }


	public async Task<User?> AddUserAsync(AddUserViewModel model)
	{
		User user = new()
		{
			Address = model.Address,
			Document = model.Document,
			Email = model.Username,
			FirstName = model.FirstName,
			LastName = model.LastName,
			ImageId = model.ImageId,
			PhoneNumber = model.PhoneNumber,
			//City = await _context.Cities.FindAsync(model.CityId),
            CityId = model.CityId,
			UserName = model.Username,
			UserType = model.UserType
		};

		IdentityResult result = await _userManager.CreateAsync(user, model.Password);
		if (result != IdentityResult.Success)
		{
			return null;
		}

		User newUser = await GetUserAsync(model.Username);
		await AddUserToRoleAsync(newUser, user.UserType.ToString());
		return newUser;
	}


	public async Task AddUserToRoleAsync(User user, string roleName)
    {
        await _userManager.AddToRoleAsync(user, roleName);
    }

    public async Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword)
    {
        return await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
    }

    public async Task CheckRoleAsync(string roleName)
    {
        bool roleExists = await _roleManager.RoleExistsAsync(roleName);
        if (!roleExists)
        {
            await _roleManager.CreateAsync(new IdentityRole
            {
                Name = roleName
            });
        }
    }

    public async Task<User> GetUserAsync(string email)
    {
        var user = await _context.Users
                .Include(u => u.City)
                .ThenInclude(c => c!.State)
                .ThenInclude(s => s!.Country)
                .FirstOrDefaultAsync(u => u.Email == email);
        return user!;
    }

    public async Task<User> GetUserAsync(Guid userId)
    {
        var user = await _context.Users
        .Include(u => u.City)
        .ThenInclude(c => c!.State)
        .ThenInclude(s => s!.Country)
        .FirstOrDefaultAsync(u => u.Id == userId.ToString());
        return user!;
    }

    public async Task<bool> IsUserInRoleAsync(User user, string roleName)
    {
        return await _userManager.IsInRoleAsync(user, roleName);

    }

    public async Task<SignInResult> LoginAsync(LoginViewModel model)
    {
        return await _singnInManager.PasswordSignInAsync(model.Username, model.Password, 
            model.RememberMe, lockoutOnFailure: true);
    }

    public async Task LogoutAsync()
    {
        await _singnInManager.SignOutAsync();
    }

    public async Task<IdentityResult> UpdateUserAsync(User user)
    {
        return await _userManager.UpdateAsync(user);
    }
}