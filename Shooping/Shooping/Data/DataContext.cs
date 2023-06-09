using Microsoft.EntityFrameworkCore;
using Shooping.Data.Entities;

namespace Shooping.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> option): base(option)
    {

    }


    public DbSet<Category> Categories { get; set; }
    public DbSet<Country> Countries { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Category>().HasIndex(c => c.Name).IsUnique();
        modelBuilder.Entity<Country>().HasIndex(c => c.Name).IsUnique();
    }

}
