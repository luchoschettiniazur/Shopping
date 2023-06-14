using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shooping.Data;
using Shooping.Data.Entities;
using Shooping.Models;

namespace Shooping.Controllers
{
    public class CountriesController : Controller
    {
        private readonly DataContext _context;

        public CountriesController(DataContext context)
        {
            _context = context;
        }

        // GET: Countries
        public async Task<IActionResult> Index()
        {
              return _context.Countries != null ? 
                     View( await _context.Countries.Include(c => c.States).ToListAsync()) 
                     : Problem("Entity set 'DataContext.Countries'  is null.");
        }

        // GET: Countries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Countries == null)
            {
                return NotFound();
            }

            var country = await _context.Countries.Include(c => c.States)!.ThenInclude(s => s.Cities)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }




        // GET: Countries/DetailsState/5
        public async Task<IActionResult> DetailsState(int? id)
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

            StateVm vm = new()
            {
                state = state
            };

            return View(vm);
        }



        // GET: Countries/Create
        public IActionResult Create()
        {
            ////Puse la lista de Country como nulleable en en la clase de Country y con eso no hace falta esto.
            //Country country = new() { States = new List<State>() };
            //return View(country);

            return View();
        }

        // POST: Countries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Name")] Country country)
        public async Task<IActionResult> Create(Country country)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(country);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbUpdateException)
                {
                    string messageError = dbUpdateException.InnerException!.Message;
                    if (messageError.Contains("duplicate") || messageError.Contains("duplicada"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe un país con el mismo nombre.");
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
            return View(country);
        }





        public async Task<IActionResult> CreateState(int? id )
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


            StateVm vm = new()
            {
                state = state
            };


            return View(vm);
        }

        //CreateState
        //AddState
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Name")] Country country)
        public async Task<IActionResult> CreateState(StateVm vm)
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

                    //he notado que enviando lo con VM si se pone a cero el Id pero por lo que sea...
                    vm.state.Id = 0;  //por lo que sea 

                    _context.Add(vm.state);

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), new {id = vm.state.CountryId});
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
            return View(vm);
        }






        public async Task<IActionResult> CreateCity(int? id)
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

            CityVm vm = new()
            {
                city = new() { StateId = state.Id }
            };

            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Name")] Country country)
        public async Task<IActionResult> CreateCity(CityVm vm)
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
                    ////PERO CUANDO UTILIZO UN VM QUE TIENE LA CLASE SI DEJA EL ID A CERO (ASI QUE POR AHORA LO COMENTO)
                    //vm.city.Id = 0;

                    _context.Add(vm.city);

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(DetailsState), new { id = vm.city.StateId });
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
            return View(vm);
        }







        // GET: Countries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Countries == null)
            {
                return NotFound();
            }

            var country = await _context.Countries.FindAsync(id);
            //como su lista de States es nuleable no es necesario buscar los estados de este pais, por eso es bueno 
            //las litas y objetos de relaciones de entity framework dejarjos nuleables con el simpbolo de pregunta ?

            if (country == null)
            {
                return NotFound();
            }
            return View(country);
        }

        // POST: Countries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Country country)
        public async Task<IActionResult> Edit(int id, Country country)
        {
            if (id != country.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(country);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
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
                        ModelState.AddModelError(string.Empty, "Ya existe un país con el mismo nombre.");
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
            return View(country);
        }



        public async Task<IActionResult> EditState(int? id)
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


            StateVm vm = new()
            {
                state = state,
            };


            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Country country)
        public async Task<IActionResult> EditState(int id, StateVm vm)
        {
            if (id != vm.state.Id)
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

                    _context.Update(vm.state);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), new { Id = vm.state.CountryId});
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
            return View(vm);
        }


        // GET: Countries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Countries == null)
            {
                return NotFound();
            }

            var country = await _context.Countries.Include(c => c.States).FirstOrDefaultAsync(c=> c.Id == id);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        // POST: Countries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Countries == null)
            {
                return Problem("Entity set 'DataContext.Countries'  is null.");
            }
            var country = await _context.Countries.FindAsync(id);
            if (country != null)
            {
                _context.Countries.Remove(country);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //private bool CountryExists(int id)
        //{
        //  return (_context.Countries?.Any(e => e.Id == id)).GetValueOrDefault();
        //}
    }
}
