using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Shooping.Models;

public class AddCategoryProductViewModel
{
    //para saber a que producto le vamos poner la categoria
    public int ProductId { get; set; }


    //la categoria que selecionamos
    [Display(Name = "Categoría")]
    [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar una categoría.")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public int CategoryId { get; set; }

    //la lista de categorias
    public IEnumerable<SelectListItem>? Categories { get; set; }

}

