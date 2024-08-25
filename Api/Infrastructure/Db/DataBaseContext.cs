using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.Entities;

namespace MinimalApi.Infrastructure.Db;

public class DataBaseContext : DbContext
{   
    public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options)
    {
        
    }

    public DbSet<Adm> Adms { get; set; } = default!;
    public DbSet<Vehicle> Vehicles { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Adm>().HasData(
            new Adm{
                Id = 1,
                Email = "adm@teste.com",
                Password = "123456",
                Profile = "Adm" 
            }
        );
    }

}