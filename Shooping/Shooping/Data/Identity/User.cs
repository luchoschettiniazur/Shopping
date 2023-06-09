﻿using Microsoft.AspNetCore.Identity;
using Shooping.Data.Entities;
using Shooping.Enums;
using System.ComponentModel.DataAnnotations;

namespace Shooping.Data.Identity;

public class User : IdentityUser
{

    [Display(Name = "Documento")]
    [MaxLength(20, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public string Document { get; set; } = null!;

    [Display(Name = "Nombres")]
    [MaxLength(50, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public string FirstName { get; set; } = null!;

    [Display(Name = "Apellidos")]
    [MaxLength(50, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public string LastName { get; set; } = null!;



    public int CityId { get; set; }

    [Display(Name = "Ciudad")]
    public City? City { get; set; }




    [Display(Name = "Dirección")]
    [MaxLength(200, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public string Address { get; set; } = null!;


    [Display(Name = "Foto")]
    public Guid ImageId { get; set; }

    //TODO: Pending to put the correct paths
    [Display(Name = "Foto")]
    public string ImageFullPath => ImageId == Guid.Empty
        ? $"https://localhost:7057/images/noimage.png"
        : $"https://sales2023storageaccount.blob.core.windows.net/users-mvc/{ImageId}";




    [Display(Name = "Tipo de usuario")]
    public UserType UserType { get; set; }



    [Display(Name = "Usuario")]
    public string FullName => $"{FirstName} {LastName}";

    [Display(Name = "Usuario")]
    public string FullNameWithDocument => $"{FirstName} {LastName} - {Document}";



}
