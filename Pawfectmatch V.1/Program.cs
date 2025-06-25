using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Pawfectmatch_V._1.Data;
using Pawfectmatch_V._1.Models;
using Pawfectmatch_V._1.Services;
using Pawfectmatch_V._1.Repositories;
using Pawfectmatch_V._1.Middleware;
using Rotativa.AspNetCore;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Microsoft.Extensions.Localization;

namespace Pawfectmatch_V._1
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container with Razor runtime compilation enabled
            builder.Services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();
            builder.Services.AddRazorPages();

            // Configure EF Core with SQLite
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Configure Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Configure login path
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Admin/Auth/Login";
            });

            // Add Memory Cache
            builder.Services.AddMemoryCache();

            // Add Localization
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[] { new CultureInfo("en"), new CultureInfo("es") };
                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });
            builder.Services.AddMvc()
                .AddViewLocalization()
                .AddDataAnnotationsLocalization();

            // Register Services
            builder.Services.AddScoped<IPetService, PetService>();
            builder.Services.AddScoped<IAdoptionService, AdoptionService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddSingleton<ILogService, LogService>();
            builder.Services.AddScoped<ICacheService, CacheService>();

            // Register EmailService for DI
            builder.Services.AddScoped<IEmailService, EmailService>();

            // Register Repositories
            builder.Services.AddScoped<IPetRepository, PetRepository>();
            builder.Services.AddScoped<IAdoptionApplicationRepository, AdoptionApplicationRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register Generic Repository
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            var app = builder.Build();

            // Middleware pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Custom middleware
            app.UseExceptionHandling();
            app.UseRequestLogging();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            RotativaConfiguration.Setup(app.Environment.WebRootPath, "Rotativa");

            // Route for Areas (e.g., Admin area)
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

            app.MapRazorPages();

            // Default route
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Seed default admin user and role
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await SeedData.Initialize(services);
            }

            // Localization middleware
            var locOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions?.Value);

            app.Run();
        }
    }
}
