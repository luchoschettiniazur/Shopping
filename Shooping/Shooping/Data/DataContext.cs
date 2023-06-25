using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Shooping.Data.Entities;
using Shooping.Data.Identity;
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
    public DbSet<State> States { get; set; }






    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Country>().HasIndex(c => c.Name).IsUnique();
        //indice no se puede repetetir un nombre de estado en el mismo pais
        modelBuilder.Entity<State>().HasIndex("Name", "CountryId").IsUnique();

        modelBuilder.Entity<City>().HasIndex("Name", "StateId").IsUnique();

    }

}
