using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Shooping.Data.Entities;

public class State
{

    public int Id { get; set; }

    [Display(Name = "Departamento/Estado")]
    [MaxLength(50, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public string Name { get; set; } = null!;


    public int CountryId { get; set; }
    //en la parte de la relacion de 1, se le puede poner [JsonIgnore] para que el los json no hagan IgnoreCycles
    //lo que hace es que en JSon esta no aparece (en esta caso [Country] se ignora en el json
    //el [JsonIgnore] no afecta al identity solo al json.
    [JsonIgnore] 
    public Country? Country { get; set; }



    public ICollection<City>? Cities { get; set; }

    //las propiedades de solo lectura no se mapean a bbdd
    [Display(Name = "Ciudades")]
    public int CitiesNumber => Cities == null ? 0 : Cities.Count;


}
