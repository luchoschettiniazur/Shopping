using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shooping.Data;
using Shooping.Data.Entities;
using System.Data;

namespace Shooping.Controllers;



[Authorize(Roles = "Admin")]
public class CategoriesController: Controller
{
    private readonly DataContext _context;

    public CategoriesController(DataContext context)
    {
        _context = context;
    }


    public async Task<IActionResult> Index()
    {
        return _context.Categories != null ?
                    View(await _context.Categories.ToListAsync()) :
                    Problem("Entity set 'DataContext.Categories'  is null.");
    }





    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.Categories == null)
        {
            return NotFound();
        }

        var category = await _context.Categories
            .FirstOrDefaultAsync(m => m.Id == id);
        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }




    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    //public async Task<IActionResult> Create([Bind("Id,Name")] Country country)
    public async Task<IActionResult> Create(Category category)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dbUpdateException)
            {
                string messageError = dbUpdateException.InnerException!.Message;
                if (messageError.Contains("duplicate") || messageError.Contains("duplicada"))
                {
                    ModelState.AddModelError(string.Empty, "Ya existe una categoría con el mismo nombre.");
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
        return View(category);

    }




    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.Categories == null)
        {
            return NotFound();
        }

        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }
        return View(category);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    //public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Country country)
    public async Task<IActionResult> Edit(int id, Category category)
    {
        if (id != category.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(category);
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
                    ModelState.AddModelError(string.Empty, "Ya existe una categoría con el mismo nombre.");
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
        return View(category);
    }




    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.Categories == null)
        {
            return NotFound();
        }

        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    // POST: Countries/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_context.Categories == null)
        {
            return Problem("Entity set 'DataContext.Categories'  is null.");
        }
        var category = await _context.Categories.FindAsync(id);
        if (category != null)
        {
            _context.Categories.Remove(category);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }





}
