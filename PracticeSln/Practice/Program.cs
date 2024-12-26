using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Practice.Data;
using Practice.Models.Item;
using Practice.Models.product;
using Serilog;
using Serilog.Events;

namespace Practice
{
    public class Program
    {
        public static void Main(string[] args)
        {
            #region bootstrap logger autofac
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateBootstrapLogger();

            #endregion

            try
            {
                Log.Information("Application starting...");
                var builder = WebApplication.CreateBuilder(args);

                // Add services to the container.
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

                #region autofac configaration

                builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
                builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
                {
                    //adding services
                    //containerBuilder.RegisterType<Item>().As<IItem>().SingleInstance(); //singleton

                    //if we make a module than - 
                    containerBuilder.RegisterModule(new WebModule());
                });

                #endregion

                #region Service Collection Dependency Injection

                builder.Services.AddKeyedScoped<IProduct, Electronics>("electronics");
                builder.Services.AddKeyedScoped<IProduct, Vhicle>("vhicle");

                #endregion

                #region serilog configaration
                builder.Host.UseSerilog((context, lc) =>
                    lc.MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .ReadFrom.Configuration(builder.Configuration)
                );
                #endregion

                builder.Services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(connectionString));
                builder.Services.AddDatabaseDeveloperPageExceptionFilter();

                builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<ApplicationDbContext>();
                builder.Services.AddControllersWithViews();

                //dependency injection
                builder.Services.AddScoped<IItem, Item>();

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseMigrationsEndPoint();
                }
                else
                {
                    app.UseExceptionHandler("/Home/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }

                app.UseHttpsRedirection();
                app.UseRouting();

                app.UseAuthorization();

                app.MapStaticAssets();
                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}")
                    .WithStaticAssets();
                app.MapRazorPages()
                   .WithStaticAssets();

                app.Run();
            }
            catch (Exception ex) 
            {
                Log.Fatal(ex, "Application crashed..");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
