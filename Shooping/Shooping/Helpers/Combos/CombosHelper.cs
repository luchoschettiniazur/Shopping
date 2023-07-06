using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shooping.Data;
using Shooping.Data.Entities;
using Shooping.Helpers.Combo;

namespace Shooping.Helpers.Combos;

public class CombosHelper : ICombosHelper
{
	private readonly DataContext _context;

	public CombosHelper(DataContext context)
    {
		_context = context;
	}

    public async Task<IEnumerable<SelectListItem>> GetComboCategoriesAsync()
	{
		List<SelectListItem> list = await _context.Categories
			.Select( cat => new SelectListItem()
			{
				Text = cat.Name,
				Value = cat.Id.ToString()
			})
			.OrderBy(sel => sel.Text)
			.ToListAsync();

		list.Insert(0, new SelectListItem() { Text = "[Seleccione una categoría...]", Value= "0" });

		return list;
	}

    public async Task<IEnumerable<SelectListItem>> GetComboCategoriesNotInProductAsync(int productId)
    {
		//Producto con su lista de categorias escogidas actualmente.
        Product? product = await _context.Products
            .Include(p => p.ProductCategories).ThenInclude(pc => pc.Category)
            .FirstOrDefaultAsync(p => p.Id == productId);


		//para hacer el filtro dentro del link tiene que ser una lista de numeros
        List<int> idsCategoriasYaElegidas = new List<int>();
        if (product is not null)
        {
			foreach (var item in product.ProductCategories)
			{
				idsCategoriasYaElegidas.Add(item.CategoryId);
            }
        }

        //lista de SelectListItem de las Categorias no escogidas actualmente en el producto
        List<SelectListItem> list = await _context.Categories
		.Where(cat => !idsCategoriasYaElegidas.Any(filt => filt == cat.Id))
		.Select(cat => new SelectListItem()
		{
			Text = cat.Name,
			Value = cat.Id.ToString()
		})
		.OrderBy(sel => sel.Text)
		.ToListAsync();

        list.Insert(0, new SelectListItem() { Text = "[Seleccione una categoría...]", Value = "0" });

        return list;
    }


    public async Task<IEnumerable<SelectListItem>> GetComboCountriesAsync()
	{
		List<SelectListItem> list = await _context.Countries
			.Select(cou => new SelectListItem()
			{
				Text = cou.Name,
				Value = cou.Id.ToString()
			})
			.OrderBy(sel => sel.Text)
			.ToListAsync();

		list.Insert(0, new SelectListItem() { Text = "[Seleccione una país...]", Value = "0" });

		return list;
	}

	public async Task<IEnumerable<SelectListItem>> GetComboStatesAsync(int countryId)
	{
		List<SelectListItem> list = await _context.States
			.Where(sta=> sta.CountryId == countryId)
			.Select(sta => new SelectListItem()
			{
				Text = sta.Name,
				Value = sta.Id.ToString()
			})
			.OrderBy(sel => sel.Text)
			.ToListAsync();

		list.Insert(0, new SelectListItem() { Text = "[Seleccione una departamento/estado...]", Value = "0" });

		return list;
	}

	public async Task<IEnumerable<SelectListItem>> GetComboCitiesAsync(int stateId)
	{
		List<SelectListItem> list = await _context.Cities
			.Where(cit => cit.StateId == stateId)
			.Select(cit => new SelectListItem()
			{
				Text = cit.Name,
				Value = cit.Id.ToString()
			})
			.OrderBy(sel => sel.Text)
			.ToListAsync();

		list.Insert(0, new SelectListItem() { Text = "[Seleccione una ciudad...]", Value = "0" });

		return list;
	}


}
