using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shooping.Data;
using Shooping.Data.Entities;
using Shooping.Models;

namespace Shooping.Controllers;
public class CitiesController : Controller
{

    private readonly DataContext _context;

    public CitiesController(DataContext context)
    {
        _context = context;
    }

    //public IActionResult Index()
    //{
    //    return View();
    //}




    public async Task<IActionResult> Create(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        State? state = await _context.States.FindAsync(id);
        if (state is null)
        {
            return NotFound();
        }


        //StateViewModel model = new()
        //{
        //    CountryId = country.Id
        //};

        //City city = new()
        //{
        //    StateId = state.Id
        //};


        City city = new() { StateId = state.Id };

        return View(city);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    //public async Task<IActionResult> Create([Bind("Id,Name")] Country country)
    public async Task<IActionResult> Create(City city)
    {
        if (ModelState.IsValid)
        {
            try
            {
                //State state = new()
                //{
                //    Cities = new List<City>(),
                //    //Country = await _context.Countries.FindAsync(model.CountryId),
                //    CountryId = model.CountryId,
                //    Name = model.Name
                //};


                ////esto se se debe a que hay un campo denominado CountryId y como finaliza con Id,
                ////tambien le da su valor a Id automaticamiente, asi que le pongo 0 ya que vamos a crearlo.
                ////esto solo pasa cuando el Id no tiene valor (osea tiene el valor) por ser un reg nuevo.
                city.Id = 0;
                ////PERO CUANDO UTILIZO UN VM QUE TIENE LA CLASE, SI DEJA EL ID A CERO (que no es el caso)

                _context.Add(city);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Shooping.Controllers.StatesController.Details),
                    "States", new { id = city.StateId });
            }
            catch (DbUpdateException dbUpdateException)
            {
                string messageError = dbUpdateException.InnerException!.Message;
                if (messageError.Contains("duplicate") || messageError.Contains("duplicada"))
                {
                    ModelState.AddModelError(string.Empty, "Ya existe una ciudad con el mismo nombre en este Deparatamento/Estado.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                }
            }
            catch (Exception exception)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
            }
        }
        return View(city);
    }





    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.Countries == null)
        {
            return NotFound();
        }

        //var state = await _context.States.Include(p => p.Country).FirstOrDefaultAsync(s => s.Id == id);
        var city = await _context.Cities.FindAsync(id);

        if (city == null)
        {
            return NotFound();
        }

        //StateViewModel model = new()
        //{
        //    //CountryId = state.Country.Id, //si hacemos el Include
        //    CountryId = state.CountryId,
        //    Id = state.Id,
        //    Name = state.Name
        //};


        return View(city);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    //public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Country country)
    public async Task<IActionResult> Edit(int id, City city)
    {
        if (id != city.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                //State state = new()
                //{
                //    Id = model.Id,
                //    Name= model.Name,
                //    CountryId = model.CountryId,
                //};

                _context.Update(city);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Shooping.Controllers.StatesController.Details), "States", new { Id = city.StateId });
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError(string.Empty, "No se ha podido modificar el registro.");
            }
            catch (DbUpdateException dbUpdateException)
            {
                string messageError = dbUpdateException.InnerException!.Message;
                if (messageError.Contains("duplicate") || messageError.Contains("duplicada"))
                {
                    ModelState.AddModelError(string.Empty, "Ya existe una ciudad con el mismo nombre en este departamento/estado.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                }
            }
            catch (Exception exception)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
            }

        }
        return View(city);
    }




    // GET: cities/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.Cities == null)
        {
            return NotFound();
        }

        var city = await _context.Cities.FindAsync(id);
        if (city == null)
        {
            return NotFound();
        }


        return View(city);
    }







    // GET: cites/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.Cities == null)
        {
            return NotFound();
        }

        var city = await _context.Cities.FindAsync(id);
        if (city == null)
        {
            return NotFound();
        }

        return View(city);
    }

    // POST: cites/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_context.Cities == null)
        {
            return Problem("Entity set 'DataContext.Cities'  is null.");
        }
        var city = await _context.Cities.FindAsync(id);
        if (city != null)
        {
            _context.Cities.Remove(city);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Details", "States", new { Id = city!.StateId });
    }









}
