using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Shooping.Data.Entities;

public class Product
{

    public int Id { get; set; }

    [Display(Name = "Nombre")]
    [MaxLength(50, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public string Name { get; set; } = null!;

    [DataType(DataType.MultilineText)]
    [Display(Name = "Descripción")]
    [MaxLength(500, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
    public string Description { get; set; } = null!;

    [Column(TypeName = "decimal(18,2)")]
    [DisplayFormat(DataFormatString = "{0:C2}")]  //para dinero ->currency con 2 decimales.
    [Display(Name = "Precio")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public decimal Price { get; set; }

    //Hemos puesto Stock con decimales, ya que podemos vender por ejempo -> 1,5 metros de alambre.
    [DisplayFormat(DataFormatString = "{0:N2}")]  //para numeros ->numerico con 2 decimales.
    [Display(Name = "Inventario")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public float Stock { get; set; }



    //public ICollection<ProductCategory>? ProductCategories { get; set; }
    public ICollection<ProductCategory> ProductCategories { get; set; } = new HashSet<ProductCategory>();

    [Display(Name = "Categorías")]
    public int CategoriesNumber => ProductCategories == null ? 0 : ProductCategories.Count;



    //public ICollection<ProductImage>? ProductImages { get; set; }
    public ICollection<ProductImage> ProductImages { get; set; } = new HashSet<ProductImage>();


    [Display(Name = "Fotos")]
    public int ImagesNumber => ProductImages == null ? 0 : ProductImages.Count;



    //OJO -> Las propidades de lectura no se mapean en BBDD.
    //TODO: Pending to change to the correct path
    [Display(Name = "Foto")]
    public string ImageFullPath => ProductImages == null || ProductImages.Count == 0
        ? $"https://localhost:7057/images/noimage.png"
        : ProductImages.FirstOrDefault()!.ImageFullPath;



}
