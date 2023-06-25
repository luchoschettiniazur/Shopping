﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shooping.Data;
using Shooping.Data.Entities;
using Shooping.Data.Identity;
using Shooping.Enums;
using Shooping.Helpers.Auth;
using Shooping.Helpers.Blob;
using Shooping.Helpers.Combo;
using Shooping.Helpers.Combos;
using Shooping.Models;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Shooping.Controllers;



public class AccountController : Controller
{
	private readonly IUserHelper _userHelper;
	private readonly DataContext _context;
	private readonly ICombosHelper _combosHelper;
	private readonly IBlobHelper _blobHelper;

	public AccountController(IUserHelper userHelper,
		DataContext context, ICombosHelper combosHelper,
		IBlobHelper blobHelper)
	{
		_userHelper = userHelper;
		_context = context;
		_combosHelper = combosHelper;
		_blobHelper = blobHelper;
	}



	public IActionResult Login()
	{
		if (User.Identity!.IsAuthenticated)
		{
			return RedirectToAction("Index", "Home");
		}

		return View(new LoginViewModel());
	}

	[HttpPost]
	public async Task<IActionResult> Login(LoginViewModel model)
	{
		if (ModelState.IsValid)
		{
			SignInResult result = await _userHelper.LoginAsync(model);
			if (result.Succeeded)
			{
				return RedirectToAction("Index", "Home");
			}

			ModelState.AddModelError(string.Empty, "Email o contraseña incorrectos.");
		}

		return View(model);
	}


	public async Task<IActionResult> Logout()
	{
		await _userHelper.LogoutAsync();
		return RedirectToAction("Index", "Home");
	}


	public IActionResult NotAuthorized()
	{
		return View();
	}




	public async Task<IActionResult> Register()
	{
		AddUserViewModel model = new()
		{
			Id = Guid.Empty.ToString(),
			Countries = await _combosHelper.GetComboCountriesAsync(),
			States = await _combosHelper.GetComboStatesAsync(0),
			Cities = await _combosHelper.GetComboCitiesAsync(0),
			UserType = UserType.User,
		};

		return View(model);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Register(AddUserViewModel addUserViewModel)
	{
		if (ModelState.IsValid)
		{
			Guid imageId = Guid.Empty;

			if (addUserViewModel.ImageFile != null)
			{
				imageId = await _blobHelper.UploadBlobAsync(addUserViewModel.ImageFile, "users-mvc");
			}

			addUserViewModel.ImageId = imageId;
			User? user = await _userHelper.AddUserAsync(addUserViewModel);
			if (user == null)
			{
				ModelState.AddModelError(string.Empty, "Este correo ya está siendo usado.");
				//recargar los combos
				addUserViewModel.Countries = await _combosHelper.GetComboCountriesAsync();
				addUserViewModel.States = await _combosHelper.GetComboStatesAsync(addUserViewModel.CountryId);
				addUserViewModel.Cities = await _combosHelper.GetComboCitiesAsync(addUserViewModel.StateId);

				return View(addUserViewModel);
			}


            //lo logueamos directamente:
            LoginViewModel loginViewModel = new LoginViewModel
            {
                Password = addUserViewModel.Password,
                RememberMe = false,
                Username = addUserViewModel.Username
            };
            var result = await _userHelper.LoginAsync(loginViewModel);


            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

        } //fin -> if (ModelState.IsValid)

        //SI LLEGA HASTA AQUI ES PORQUE NO ES VALIDO EL ModelState:
        //recargar los combos
        addUserViewModel.Countries = await _combosHelper.GetComboCountriesAsync();
        addUserViewModel.States = await _combosHelper.GetComboStatesAsync(addUserViewModel.CountryId);
        addUserViewModel.Cities = await _combosHelper.GetComboCitiesAsync(addUserViewModel.StateId);
        return View(addUserViewModel);
	}





    public async Task<IActionResult> ChangeUser()
    {
        User user = await _userHelper.GetUserAsync(User.Identity!.Name!);
        if (user == null)
        {
            return NotFound();
        }

        EditUserViewModel model = new()
        {
            Address = user.Address,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber!,
            ImageId = user.ImageId,
            Cities = await _combosHelper.GetComboCitiesAsync(user.City!.State!.Id),
            CityId = user.City.Id,
            Countries = await _combosHelper.GetComboCountriesAsync(),
            CountryId = user.City.State.Country!.Id,
            StateId = user.City.State.Id,
            States = await _combosHelper.GetComboStatesAsync(user.City.State.Country.Id),
            Id = user.Id,
            Document = user.Document
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeUser(EditUserViewModel model)
    {
        if (ModelState.IsValid)
        {
            Guid imageId = model.ImageId;

            if (model.ImageFile != null)
            {
                imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users-mvc");
            }

            User user = await _userHelper.GetUserAsync(User.Identity!.Name!);

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Address = model.Address;
            user.PhoneNumber = model.PhoneNumber;
            user.ImageId = imageId;
            //user.City = await _context.Cities.FindAsync(model.CityId);
            user.CityId = model.CityId;

            user.Document = model.Document;
            await _userHelper.UpdateUserAsync(user);
            return RedirectToAction("Index", "Home");
        }

        //SI LLEGA HASTA AQUI ES PORQUE NO ES VALIDO EL ModelState:
        //recargar los combos
        model.Countries = await _combosHelper.GetComboCountriesAsync();
        model.States = await _combosHelper.GetComboStatesAsync(model.CountryId);
        model.Cities = await _combosHelper.GetComboCitiesAsync(model.StateId);
        return View(model);
    }





    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordUserViewModel model)
    {
        if (ModelState.IsValid)
        {

            if(model.OldPassword == model.NewPassword)
            {
                ModelState.AddModelError(string.Empty, "Debes ingresar una contraseña distinta.");
                return View(model);
            }


            var user = await _userHelper.GetUserAsync(User.Identity!.Name!);
            if (user != null)
            {
                IdentityResult result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    return RedirectToAction("ChangeUser");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault()!.Description);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Usuario no encontrado.");
            }
        }

        return View(model);
    }













    #region Helpers Ajax
    //Los metodo que se llaman por ajax tienen que ser publicos


    public JsonResult? GetStates(int countryId)
    {
        Country? country = _context.Countries
            .Include(c => c.States)
            .FirstOrDefault(c => c.Id == countryId);
        if (country == null)
        {
            return null;
        }

		var jasonx = Json(country.States!.OrderBy(d => d.Name));

		return jasonx;
    }

    public JsonResult? GetCities(int stateId)
    {
        State? state = _context.States
            .Include(s => s.Cities)
            .FirstOrDefault(s => s.Id == stateId);
        if (state == null)
        {
            return null;
        }

        return Json(state.Cities!.OrderBy(c => c.Name));
    }


    #endregion










}


