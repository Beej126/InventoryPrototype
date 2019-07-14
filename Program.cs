using InventoryPrototype.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryPrototype {
  public class Program {
    public static void Main(string[] args) {
      var host = CreateWebHostBuilder(args).Build();

      //https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/working-with-sql?view=aspnetcore-2.2#add-the-seed-initializer
      //using (var serviceScope = host.Services.CreateScope())
      //{
      //  var serviceProvider = serviceScope.ServiceProvider;
      //  var dbContext = serviceProvider.GetRequiredService<InventoryDbContext>();
      //  dbContext.Database.Migrate(); //another way to create that is migrations compatible vs this.Database.EnsureCreated() in InventoryDBContext.cs
      //}
      host.Run();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>();
  }
}
