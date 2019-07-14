using Microsoft.EntityFrameworkCore;
using System;
using InventoryPrototype.Models;

namespace InventoryPrototype
{

  //tutorial on full api vertical slice including EF
  //https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-2.2&tabs=visual-studio
  //adding in LocalDB vs in memory db
  //https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/working-with-sql?view=aspnetcore-2.2
  public class InventoryDbContext : DbContext
  {
    public InventoryDbContext(DbContextOptions options) : base(options)
    {
      Database.EnsureCreated();
    }

    public DbSet<InventoryItem> Items { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      //modelBuilder.Entity<InventoryItem>().HasData();
    }
  }
}