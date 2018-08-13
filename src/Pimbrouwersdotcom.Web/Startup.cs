using FluentValidation;
using FluentValidation.AspNetCore;
using LunchPail;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pimbrouwersdotcom.Data;
using Pimbrouwersdotcom.Domain;
using Pimbrouwersdotcom.Web.Services;
using Sequel;
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

      //context
      services.AddTransient<IDbConnectionFactory>(_ =>
      {
        return new DbConnectionFactory(() =>
        {
          var conn = new SqliteConnection(Configuration.GetConnectionString("DefaultConnection"));
          conn.Open();

          return conn;
        });
      });
      services.AddScoped<IDbContext, DbContext>();

      //validators
      services.AddTransient<IValidator<Post>, PostValidator>();
      services.AddTransient<IValidator<Tag>, TagValidator>();

      //entity maps
      services.AddSingleton(typeof(ISqlMapper<>), typeof(SqlMapper<>));

      //repositories
      services.AddScoped<AccountRepository>();
      services.AddScoped<PostRepository>();
      services.AddScoped<TagRepository>();

      //services
      services.AddScoped<PostService>();
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