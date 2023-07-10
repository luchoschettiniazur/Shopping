using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Shooping.Data.Entities;
using Shooping.Data.Identity;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Shooping.Data;

public class DataContext : IdentityDbContext<User>
{
    public DataContext(DbContextOptions<DataContext> option): base(option)
    { 

    }
//    Severity Code    Description Project File Line    Suppression State
//Error CS0234  The type or namespace name 'User' does not exist in the namespace 'Shooping.Data.Entities'
//(are you missing an assembly reference?)	Shooping
//C:\____Proyectos\ZULU\MVC\Curso 68\Shooping\Shooping\
//Microsoft.NET.Sdk.Razor.SourceGenerators\Microsoft.NET.Sdk.Razor.SourceGenerators.RazorSourceGenerator\
//Views_Users_Index_cshtml.g.cs	32	Active


    public DbSet<Category> Categories { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }

    public DbSet<State> States { get; set; }

    public DbSet<TemporalSale> TemporalSales { get; set; }






    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Country>().HasIndex(c => c.Name).IsUnique();
        //indice no se puede repetetir un nombre de estado en el mismo pais
        modelBuilder.Entity<State>().HasIndex("Name", "CountryId").IsUnique();

        modelBuilder.Entity<City>().HasIndex("Name", "StateId").IsUnique();
        //OJO (MUY INTERESANTE LO QUE VIENE COMENTADO)
        ////Asi podriamos hacer que no tenga en cuenta los registros en estdo false para el indice unico,
        ////si habiera un ya un nombre repetido pero en estado false no lo tendira en cuenta para el indice unico.
        //modelBuilder.Entity<City>().HasIndex("Name", "StateId").IsUnique().HasFilter("NomCampoEstado = 'false'");

        modelBuilder.Entity<Product>().HasIndex(c => c.Name).IsUnique();
        modelBuilder.Entity<ProductCategory>().HasIndex("ProductId", "CategoryId").IsUnique();
    }

}
