using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryPrototype {
  public class Startup {
    public Startup(IConfiguration configuration) {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
      services.AddMvc(options => {
        options.Filters.Add(new ApiExceptionFilter());
      })
        .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

      // these files get created here by default:
      // file://%localappdata%\Microsoft\Microsoft%20SQL%20Server%20Local%20DB\Instances\
      var connection = @"Server=(localdb)\MSSQLLocalDB;Database=Inventory;Trusted_Connection=True;MultipleActiveResultSets=true";
      services.AddDbContext<InventoryDbContext>(options => options.UseSqlServer(connection));
      // In production, the React files will be served from this directory
      services.AddSpaStaticFiles(configuration => {
        configuration.RootPath = "ClientApp/build";
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
      if (env.IsDevelopment()) {
        app.UseDeveloperExceptionPage();
      }
      else {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
      }

      app.UseHttpsRedirection();
      app.UseStaticFiles();
      app.UseSpaStaticFiles();

      app.UseMvc(routes => {
        routes.MapRoute(
            name: "default",
            template: "api/{controller}/{action=Index}/{id?}");
      });

      app.UseSpa(spa => {
        spa.Options.SourcePath = "ClientApp";

        if (env.IsDevelopment()) {
          spa.UseReactDevelopmentServer(npmScript: "start");
        }
      });
    }
  }
}
