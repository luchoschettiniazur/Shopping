using Microsoft.AspNetCore.Mvc.Rendering;
using Shooping.Data.Entities;

namespace Shooping.Helpers.Combo;

public interface ICombosHelper
{
	Task<IEnumerable<SelectListItem>> GetComboCategoriesAsync();


	Task<IEnumerable<SelectListItem>> GetComboCategoriesNotInProductAsync(int productId);



	Task<IEnumerable<SelectListItem>> GetComboCountriesAsync();


	//el countryId es el pais en el cual hay que buscar los departamentos.
	Task<IEnumerable<SelectListItem>> GetComboStatesAsync(int countryId);


	//el stateId es el estado en el cual hay que buscar las ciudades.
	Task<IEnumerable<SelectListItem>> GetComboCitiesAsync(int stateId);


}
