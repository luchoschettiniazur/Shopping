using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Shooping.Models;

public class CreateProductViewModel : EditProductViewModel
{
    //HEREDA DE EditProductViewModel (QUE SON TODOS LOS CAMPOS DE EDICION)


    //PARA UNA LISTA DE CATEGORIAS Y ES OBLIGATORIO SELECCIONAR UN CATEGORIA
    [Display(Name = "Categoría")]
    [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar una categoría.")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public int CategoryId { get; set; }

    public IEnumerable<SelectListItem>? Categories { get; set; }



    //PARA GURARDAR UNA IMAGEN (PUEDE SER NULL)
    [Display(Name = "Foto")]
    public IFormFile? ImageFile { get; set; }



    //MAS ADELANTE VOY A PODER AÑADIR MAS DE UNA FOTO AL PRODUCTO...


}
