﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shooping.Data.Entities;
using Shooping.Data;
using Shooping.Enums;
using Shooping.Helpers.Auth;
using Shooping.Helpers.Blob;
using Shooping.Helpers.Combo;
using Shooping.Models;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Shooping.Data.Identity;
using Shooping.Helpers.Email;
using Shooping.Common;

namespace Shooping.Controllers;

[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly IUserHelper _userHelper;
    private readonly DataContext _context;
    private readonly ICombosHelper _combosHelper;
    private readonly IBlobHelper _blobHelper;
    private readonly IMailHelper _mailHelper;

    public UsersController(IUserHelper userHelper, DataContext context, 
        ICombosHelper combosHelper, 
        IBlobHelper blobHelper,
        IMailHelper mailHelper)
    {
        _userHelper = userHelper;
        _context = context;
        _combosHelper = combosHelper;
        _blobHelper = blobHelper;
        _mailHelper = mailHelper;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Users
            .Include(u => u.City!)
            .ThenInclude(c => c.State!)
            .ThenInclude(s => s.Country!)
            .ToListAsync());
    }


    //OJO:
    //Este controller UserController es para adminsitradores
    //- lista de usuarios (index) con opciones de adminstrador.
    //- crear administradores


    //Ojo para que seapas En el accountcontroller es el metodo Register()
    public async Task<IActionResult> Create()
    {
        AddUserViewModel model = new AddUserViewModel
        {
            Id = Guid.Empty.ToString(),
            Countries = await _combosHelper.GetComboCountriesAsync(),
            States = await _combosHelper.GetComboStatesAsync(0),
            Cities = await _combosHelper.GetComboCitiesAsync(0),
            UserType = UserType.Admin,
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AddUserViewModel addUserViewModel)
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



            //Para enviar email para confirmacion de creacion de usuario:
            //********************************>>>
            string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
            string tokenLink = Url.Action("ConfirmEmail", "Account", new
            {
                userid = user.Id,
                token = myToken
            }, protocol: HttpContext.Request.Scheme)!;

            Response response = _mailHelper.SendMail(
                toName: $"{addUserViewModel.FirstName} {addUserViewModel.LastName}",
                toEmail: addUserViewModel.Username,
                subject: "Shopping - Confirmación de Email",
                body: $"<h1>Shopping - Confirmación de Email</h1>" +
                    $"<p>Para habilitar el usuario, por favor hacer clic 'Confirmar Email':</p>" +
                    $"<b><a href ={tokenLink}>Confirmar Email</a></b>");
            if (response.IsSuccess)
            {
                ViewBag.Message = "Las instrucciones para habilitar el administrador han sido enviadas al correo.";
                return View(addUserViewModel);
            }

            ModelState.AddModelError(string.Empty, response.Message!);
            //********************************<<<


            //return RedirectToAction("Index", "Home");

        } //fin -> if (ModelState.IsValid)


        //SI LLEGA HASTA AQUI ES PORQUE NO ES VALIDO EL ModelState:
        //recargar los combos
        addUserViewModel.Countries = await _combosHelper.GetComboCountriesAsync();
        addUserViewModel.States = await _combosHelper.GetComboStatesAsync(addUserViewModel.CountryId);
        addUserViewModel.Cities = await _combosHelper.GetComboCitiesAsync(addUserViewModel.StateId);
        return View(addUserViewModel);
    }




    //public JsonResult? GetStates(int countryId)
    //{
    //    Country? country = _context.Countries
    //        .Include(c => c.States)
    //        .FirstOrDefault(c => c.Id == countryId);
    //    if (country == null)
    //    {
    //        return null;
    //    }

    //    return Json(country.States.OrderBy(d => d.Name));
    //}





    #region Helpers
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
