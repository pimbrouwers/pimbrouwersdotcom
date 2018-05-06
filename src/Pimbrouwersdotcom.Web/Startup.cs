using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pimbrouwersdotcom.Data;
using Pimbrouwersdotcom.Domain;
using WebMarkupMin.AspNetCore2;

namespace Pimbrouwersdotcom.Web
{
  public class Startup
  {
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
      services
        .AddAuthentication("pimbrouwersdotcom")
        .AddCookie("pimbrouwersdotcom", (options) =>
        {
          options.LoginPath = "/Admin/Account/Login";
        });

      services
        .AddWebMarkupMin()
        .AddHtmlMinification();

      services
        .AddMvc()
        .AddFluentValidation(options =>
        {
          options.ImplicitlyValidateChildProperties = true;
        });

      services.AddScoped<IDbConnectionFactory>(options =>
      {
        return new SqliteConnectionFactory(Configuration.GetConnectionString("DefaultConnection"));
      });
      services.AddScoped<DbContext>();
      services.AddTransient<IValidator<Post>, PostValidator>();
      services.AddTransient<IValidator<Tag>, TagValidator>();
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      app.UseAuthentication();

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseWebMarkupMin();
        app.UseExceptionHandler("/error");
      }

      app.UseStatusCodePagesWithReExecute("/error/code/{0}");
      app.UseStaticFiles();

      app.UseMvc(routes =>
      {
        routes.MapRoute(
          "admin",
          "{area}/{controller=Post}/{action=Index}/{id?}");

        routes.MapRoute(
          "default",
          "{controller=Post}/{action=Index}/{id?}");
      });
    }
  }
}