﻿using Microsoft.AspNetCore.Mvc;
using Shooping.Helpers.Auth;
using Shooping.Models;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Shooping.Controllers;



public class AccountController : Controller
{
    private readonly IUserHelper _userHelper;

    public AccountController(IUserHelper userHelper)
    {
        _userHelper = userHelper;
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



}


