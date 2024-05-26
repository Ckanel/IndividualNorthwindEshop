using CommonData.Data;
using Microsoft.EntityFrameworkCore;
using IndividualNorthwindEshop.Services;
using CommonData.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using ETL.Extract;
using ETL.Transform;
using ETL.Load;
using System.Diagnostics;
using ETL.Transform.DataAccess;
using System.Configuration;
namespace IndividualNorthwindEshop
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args);

                // Add services to the container.
                builder.Services.AddControllersWithViews();
                builder.Services.AddRazorPages();
                builder.Services.AddDbContext<MasterContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("Northwind")));
                builder.Services.AddIdentity<User, IdentityRole>()
                    .AddEntityFrameworkStores<MasterContext>().AddDefaultTokenProviders();
                builder.Services.AddSingleton<IEmailSender, EmailSender>();
                builder.Services.AddScoped<IOle78DecryptionService, Ole78DecryptionService>();
                builder.Services.AddSession(options =>
                {
                    options.IdleTimeout = TimeSpan.FromMinutes(30);
                    options.Cookie.HttpOnly = true;
                    options.Cookie.IsEssential = true;
                });
                builder.Services.AddLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddDebug();
                });
                builder.Services.AddScoped<CartService>();
                builder.Services.AddScoped<OrderService>();
                builder.Services.AddScoped<IPaginationService, PaginationService>();
                builder.Services.Configure<ConnectionStrings>(builder.Configuration.GetSection("ConnectionStrings"));

                // Register ETL services
                builder.Services.AddScoped<OrderRepository>();
                builder.Services.AddScoped<OrderTransform>();
                builder.Services.AddScoped<ETLProcess>();
                string connectionString = builder.Configuration.GetConnectionString("ETLDatabase");
                builder.Services.AddSingleton(new ETLDataExportService(connectionString));
                builder.Services.AddScoped(sp => new LoadProcess(connectionString));

                builder.Services.AddHostedService<ETLBackgroundService>();
                var app = builder.Build();

                using (var scope = app.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                    // Create the roles if they don't exist
                    string[] roleNames = { "Customer", "Employee", "Manager" };
                    foreach (var roleName in roleNames)
                    {
                        if (!await roleManager.RoleExistsAsync(roleName))
                        {
                            await roleManager.CreateAsync(new IdentityRole(roleName));
                        }
                    }
                }

                // Configure the HTTP request pipeline.
                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Home/Error");
                    app.UseHsts();
                }

                app.UseHttpsRedirection();
                app.UseStaticFiles();

                app.UseRouting();
                app.UseSession();
                app.UseAuthorization();

                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                app.MapRazorPages();
                app.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unhandled exception: {ex}");
                throw;
            }
        }


    }
}

