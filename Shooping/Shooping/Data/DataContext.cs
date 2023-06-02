using Microsoft.EntityFrameworkCore;
using Shooping.Data.Entities;

namespace Shooping.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> option): base(option)
    {

    }

    public DbSet<Country> Countries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Country>().HasIndex(c => c.Name).IsUnique();
    }

}
