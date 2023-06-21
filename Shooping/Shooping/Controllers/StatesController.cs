using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shooping.Data;
using Shooping.Data.Entities;
using Shooping.Models;

namespace Shooping.Controllers;
public class StatesController : Controller
{
    private readonly DataContext _context;

    public StatesController(DataContext context)
    {
        _context = context;
    }









    public async Task<IActionResult> Create(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        Country? country = await _context.Countries.FindAsync(id);
        if (country is null)
        {
            return NotFound();
        }

        //StateViewModel model = new()
        //{
        //    CountryId = country.Id
        //};

        State state = new()
        {
            CountryId = country.Id
        };


        return View(state);
    }

    //CreateState
    //AddState
    [HttpPost]
    [ValidateAntiForgeryToken]
    //public async Task<IActionResult> Create([Bind("Id,Name")] Country country)
    public async Task<IActionResult> Create(State state)
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


                //esto se se debe a que hay un campo denominado CountryId y como finaliza con Id,
                //tambien le da su valor a Id automaticamiente, asi que le pongo 0 ya que vamos a crearlo.
                //esto solo pasa cuando el Id no tiene valor (osea tiene el valor) por ser un reg nuevo.

                //he notado que enviando lo con VM si se pone a cero el Id (pero este no es el caso)
                state.Id = 0;  //por lo que sea 

                _context.Add(state);

                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Countries", new { id = state.CountryId });
            }
            catch (DbUpdateException dbUpdateException)
            {
                string messageError = dbUpdateException.InnerException!.Message;
                if (messageError.Contains("duplicate") || messageError.Contains("duplicada"))
                {
                    ModelState.AddModelError(string.Empty, "Ya existe un departamento/estado con el mismo nombre en este país.");
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
        return View(state);
    }











    // GET: states/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.States == null)
        {
            return NotFound();
        }

        var state = await _context.States.Include(s => s.Cities)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (state == null)
        {
            return NotFound();
        }


        return View(state);
    }





    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.Countries == null)
        {
            return NotFound();
        }

        //var state = await _context.States.Include(p => p.Country).FirstOrDefaultAsync(s => s.Id == id);
        var state = await _context.States.FindAsync(id);

        if (state == null)
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


        return View(state);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    //public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Country country)
    public async Task<IActionResult> Edit(int id, State state)
    {
        if (id != state.Id)
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

                _context.Update(state);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Shooping.Controllers.CountriesController.Details), "Countries", new { Id = state.CountryId });
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
                    ModelState.AddModelError(string.Empty, "Ya existe un departamento/estado con el mismo nombre en este país.");
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
        return View(state);
    }







    // GET: states/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.States == null)
        {
            return NotFound();
        }

        var state = await _context.States.FindAsync(id);
        if (state == null)
        {
            return NotFound();
        }

        return View(state);
    }

    // POST: states/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_context.States == null)
        {
            return Problem("Entity set 'DataContext.States'  is null.");
        }
        var state = await _context.States.FindAsync(id);
        if (state != null)
        {
            _context.States.Remove(state);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Details","Countries", new { Id = state!.CountryId } );
    }


    //https://localhost:7197/Counties/Details/1
    //https://localhost:7197/Countries/Details/1




}
