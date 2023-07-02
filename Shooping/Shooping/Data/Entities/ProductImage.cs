using System.ComponentModel.DataAnnotations;

namespace Shooping.Data.Entities;

//por el nombre de la entidad parece (una entidad para poder hacer de muchos a muchos) pero no es asi
//esta entidad es en si la entidad de imagenes de la los productos 
//esta entidad es 1 imagen que pertenece a 1 producto (Los productos pueden tener muchas imagenes).
public class ProductImage
{
    public int Id { get; set; }


    //Es para guardar el guid (del nombre de la imagen de azure storage)
    [Display(Name = "Foto")]
    public Guid ImageId { get; set; }

    //TODO: Pending to change to the correct path
    [Display(Name = "Foto")]
    public string ImageFullPath => ImageId == Guid.Empty
        ? $"https://localhost:7057/images/noimage.png"
        : $"https://sales2023storageaccount.blob.core.windows.net/products-mvc/{ImageId}";




    public int ProductId { get; set; }
    public Product? Product { get; set; }

}
