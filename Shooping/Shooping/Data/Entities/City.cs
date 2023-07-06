using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Shooping.Data.Identity;

namespace Shooping.Data.Entities;

public class City
{
    public int Id { get; set; }

    [Display(Name = "Ciudad")]
    [MaxLength(50, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public string Name { get; set; } = null!;



    public int StateId { get; set; }
    //en la parte de la relacion de 1, se le puede poner [JsonIgnore] para que el los json no hagan IgnoreCycles
    //lo que hace es que en JSon esta no aparece (en esta caso [State] se ignora en el json
    //el [JsonIgnore] no afecta al identity solo al json.
    [JsonIgnore]
    public State? State { get; set; }


    public ICollection<User> Users { get; set; } = new HashSet<User>();

}
