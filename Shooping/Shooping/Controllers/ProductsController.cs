using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shooping.Data;
using Shooping.Data.Entities;
using Shooping.Helpers.Blob;
using Shooping.Helpers.Combo;
using Shooping.Helpers.Combos;
using Shooping.Models;

namespace Shooping.Controllers;

[Authorize(Roles = "Admin")]
public class ProductsController : Controller
{
    private readonly DataContext _context;
    private readonly ICombosHelper _combosHelper;
    private readonly IBlobHelper _blobHelper;



    public ProductsController(DataContext context,
        ICombosHelper combosHelper, IBlobHelper blobHelper)
    {
        _context = context;
        _combosHelper = combosHelper;
        _blobHelper = blobHelper;
    }


    public async Task<IActionResult> Index()
    {
        return _context.Products != null ?
                    View(await _context.Products
                    .Include(p => p.ProductImages)
                    .Include(p => p.ProductCategories)!.ThenInclude(c => c.Category)
                    .ToListAsync()) :
                    Problem("Entity set 'DataContext.Products'  is null.");
    }




    //Mouse Gamer de 4 botónes
    public async Task<IActionResult> Create()
    {
        CreateProductViewModel model = new()
        {
            Categories = await _combosHelper.GetComboCategoriesAsync(),
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProductViewModel model)
    {
        if (ModelState.IsValid)
        {
            Guid imageId = Guid.Empty;
            if (model.ImageFile != null)
            {
                imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "products-mvc");
            }

            Product product = new()
            {
                Description = model.Description,
                Name = model.Name,
                Price = model.Price,
                Stock = model.Stock,
            };


            //añadir categoria al producto, esto tambien se puede hacer de otras maneras
            //por ejemplo:
            //añadiendo en la tabla de ProductCategories uno nuevo ProductCategory con el ProductoId y el CategoryId
            product.ProductCategories = new List<ProductCategory>()
            {
                new ProductCategory
                {
                    Category = await _context.Categories.FindAsync(model.CategoryId)
                }
            };


            //significa que selecciono imagen
            if (imageId != Guid.Empty)
            {
                //añadir imagen al producto, esto tambien se puede hacer de otras maneras
                //por ejemplo:
                //añadiendo en la tabla de ProductImages uno nuevo ProductImage con el ProductoId y el ImageId
                product.ProductImages = new List<ProductImage>()
                {
                    new ProductImage { ImageId = imageId }
                };
            }


            try
            {
                //como estamos ligando la categoria y la imagen al producto, asi cuando agregemos el producto 
                //ya agregara la categoria y la imgen (o se hace todo o no se hace), eso es lo bueno de hacerlo
                //asi, y no añadiendo por separado todo categoria, imagen y producto
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dbUpdateException)
            {
                //if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
                string messageError = dbUpdateException.InnerException!.Message;
                if (messageError.Contains("duplicate") || messageError.Contains("duplicada"))
                {
                    ModelState.AddModelError(string.Empty, "Ya existe un producto con el mismo nombre.");
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

        model.Categories = await _combosHelper.GetComboCategoriesAsync();
        return View(model);
    }






}
