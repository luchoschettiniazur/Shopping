using Shooping.Data.Identity;
using System.ComponentModel.DataAnnotations;

namespace Shooping.Data.Entities;

public class TemporalSale
{

    public int Id { get; set; }


    public string UserId { get; set; } = null!;
    public User User { get; set; } = null!;


    public int ProductId { get; set; }
    public Product? Product { get; set; }


    [DisplayFormat(DataFormatString = "{0:N2}")]
    [Display(Name = "Cantidad")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public float Quantity { get; set; }

    [DataType(DataType.MultilineText)]
    [Display(Name = "Comentarios")]
    public string? Remarks { get; set; }


}
