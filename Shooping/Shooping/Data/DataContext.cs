using Microsoft.EntityFrameworkCore;
using Shooping.Data.Entities;

namespace Shooping.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> option): base(option)
    {

    }


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

    }

}
