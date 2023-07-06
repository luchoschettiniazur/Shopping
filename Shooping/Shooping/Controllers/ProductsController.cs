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









    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        Product? product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        EditProductViewModel model = new()
        {
            Description = product.Description,
            Id = product.Id, //no se lo pedimos al usuario,pero lo necesitamos para el post
            Name = product.Name,
            Price = product.Price,
            Stock = product.Stock,
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CreateProductViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        try
        {
            Product? product = await _context.Products.FindAsync(model.Id);
            if (product is null) return NotFound();
            product.Description = model.Description;
            product.Name = model.Name;
            product.Price = model.Price;
            product.Stock = model.Stock;
            _context.Update(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException dbUpdateException)
        {
            //if (dbUpdateException.InnerException.Message.Contains("duplicate"))
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

        return View(model);
    }




    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        Product? product = await _context.Products
            .Include(p => p.ProductImages)
            .Include(p => p.ProductCategories)!.ThenInclude(pc => pc.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }






    public async Task<IActionResult> AddImage(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        Product? product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        AddProductImageViewModel model = new()
        {
            ProductId = product.Id,
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddImage(AddProductImageViewModel model)
    {
        if (ModelState.IsValid)
        {
            ////Guid imageId = Guid.Empty;

            ////como el campo es requerido en el modelo, no haca falta y lo comentamos.
            //if (model.ImageFile != null)
            //{
            //    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "products");
            //}

            Guid imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "products-mvc");



            Product? product = await _context.Products.FindAsync(model.ProductId);
            if (product == null)
            {
                return NotFound(); //no encuetra el producto.
            }
            ProductImage productImage = new()
            {
                //Product = product,   -> tambien funciona pero prefiero pasarle el Id del producto 
                ProductId = product.Id,
                ImageId = imageId,
            };




            try
            {
                _context.Add(productImage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { Id = product.Id });
            }
            catch (Exception exception)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
            }
        }

        return View(model);
    }



    //esta entidad ProductImage (no es una realcion de muchos a muchos) esta entidad en sus parametros
    //ProductId, ImageId (ya guarda el producto  y la imagen que tiene) un producto puede tener muchas imagenes.
    //Este parametro es el id ->es el id de ProductImage  (no es el id de de la imagen, el id de la imagen es el ImageId)
    //osea si se elmimna este registro se elmina la relacion entre el (producto y la categoria)
    public async Task<IActionResult> DeleteImage(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        //necesitamos el product para saber a que producto se le elmino la imagen,ya despues de
        //eliminar nos volvemos a detalles de producto (por eso necesitamos saber que producto es)
        ProductImage? productImage = await _context.ProductImages
            .Include(pi => pi.Product)
            .FirstOrDefaultAsync(pi => pi.Id == id);
        if (productImage == null)
        {
            return NotFound();
        }

        await _blobHelper.DeleteBlobAsync(productImage.ImageId, "products-mvc");
        _context.ProductImages.Remove(productImage);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { Id = productImage.Product!.Id });
    }







    public async Task<IActionResult> AddCategory(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        Product? product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }


        AddCategoryProductViewModel model = new()
        {
            ProductId = product.Id,
            //Categories = await _combosHelper.GetComboCategoriesAsync(), (antes todos)
            Categories = await _combosHelper.GetComboCategoriesNotInProductAsync(id.Value),
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddCategory(AddCategoryProductViewModel model)
    {
        if (ModelState.IsValid)
        {

            //ESTAS 2 COMPROBACIONES, las podriamos dejar de hacer ya que se supone
            //que el producto y categoria existen ya que se validaron el GET pero
            //por si nos jaquean jajajaja.
            Product? product = await _context.Products.FindAsync(model.ProductId);
            if (product == null)
            {
                return NotFound();
            }
            Category? category = await _context.Categories.FindAsync(model.CategoryId);
            if (category == null)
            {
                return NotFound();
            }

            //// ESTO LO PODRIA HACER DE LAS 2 MANERAS:
            //ProductCategory productCategory = new()
            //{
            //    Category = category,
            //    Product = product,
            //};
            ProductCategory productCategory = new()
            {
                CategoryId = model.CategoryId,
                ProductId = model.ProductId,
            };


            try
            {
                _context.Add(productCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { Id = product.Id });
            }
            catch (Exception exception)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
            }
        }

        //model.Categories = await _combosHelper.GetComboCategoriesAsync();    //antes buscaba todos
        model.Categories = await _combosHelper.GetComboCategoriesNotInProductAsync(model.ProductId);
        return View(model);
    }






    //este parametro id es el de ProductCategory  (no es ni el id de producto, ni el id de categoria)
    //osea si se elmina este registro se elmina la relacion entre el (producto y la categoria)
    public async Task<IActionResult> DeleteCategory(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        ProductCategory? productCategory = await _context.ProductCategories
            //.Include(pc => pc.Product)   -> no hace falta ya que tenemos el ProductId
            .FirstOrDefaultAsync(pc => pc.Id == id);
        if (productCategory == null)
        {
            return NotFound();
        }

        _context.ProductCategories.Remove(productCategory);
        await _context.SaveChangesAsync();
        //return RedirectToAction(nameof(Details), new { Id = productCategory.Product.Id });
        return RedirectToAction(nameof(Details), new { Id = productCategory.ProductId });
    }











    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }


            //como vamos a poner en la vista la cantidad de Catategorias y Fotos
            //tenemos que incluir la tablas de  ProductImages y ProductCategories  a productos
            //para poder obtener los totales de sus propiedades de lectura->  ImagesNumber y  CategoriesNumber

        Product? product = await _context.Products
            .Include(p => p.ProductCategories)
            .Include(p => p.ProductImages)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Product productPost)
    {
        //NO hacemos comprobacion del ModelState.IsValid (ya que solo nos va a llegar el id del producto)
        //y no pasaria.



        //CONCEPTO IMPORTANTE DE ELMINACION POR FOREIGN KEY  (con eliminacion den cascada que es lo predeterminado)
        //***********************************************************************************>>>
        //NO ES NECESARIO incluir el ProductCategories, para que las elimine tambien SUS registros relacionados
        //ya que SI SE ELIMINAN SUS REG. (YA QUE TIENE REALACION FOREIGN KEY CON LA TABLA DE PRODUCTOS) 
        Product? product = await _context.Products
        //.Include(p => p.ProductCategories)    //OJO SI SE ELIMINAN LOS REGISTROS ProductCategories SIN PONER LA TABLA (YA QUE TIENE REALACION FOREIGN KEY CON PRODUCTOS)
        .Include(p => p.ProductImages)  //ESTE si lo incluyo para pode elinar las imagens en un foreach  (PERO NO HACE FALTA PARA bbdd, se eliminario igual PORQUE TIENE RELACION)
        .FirstOrDefaultAsync(p => p.Id == productPost.Id);

        //EJEMPLOS:
        //SI SE ELIMINAN los registros de las tablas que tengan FOREIGN KEY de la tabla de Products en ellas:
        //  Tabla->[ProductCategories]  FOREIGN KEY->[ProductId]     SI SE ELIMINA el reg. si elimino el producto con el id xxx
        //  Tabla->[ProductImages]      FOREIGN KEY->[ProductId]     SI SE ELIMINA el reg. si elimino el producto con el id xxx

        //NO SE ELIMINA el reg. de BBDD (esta tabla no tiene un FOREIGN KEY->[ProductId]) den la tabla Products
        //  Tabla->[Categories]         tiene una lista a [ProductCategories] pero no un un foreinkey->[ProductId] 
        //***********************************************************************************<<<



        if (product == null)
        {
            return NotFound();
        }

        //primero borra los registros de bbdd
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        //y luego las imagenes (es por si falla la borrada en bbdd no quedarme sin imagenes)
        if (product.ProductImages is not null)
        {
            foreach (ProductImage productImage in product.ProductImages)
            {
                await _blobHelper.DeleteBlobAsync(productImage.ImageId, "products");
            }
        }

        return RedirectToAction(nameof(Index));
    }







}
