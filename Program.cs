using IndividualNorthwindEshop.Data;
using Microsoft.EntityFrameworkCore;
using IndividualNorthwindEshop.Services;
using IndividualNorthwindEshop.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;

namespace IndividualNorthwindEshop
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
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
                var app = builder.Build();
                using (var scope = app.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                    // Create the "Customer" role if it doesn't exist
                    if (!await roleManager.RoleExistsAsync("Customer"))
                    {
                        await roleManager.CreateAsync(new IdentityRole("Customer"));
                    }

                    // Create the "Employee" role if it doesn't exist
                    if (!await roleManager.RoleExistsAsync("Employee"))
                    {
                        await roleManager.CreateAsync(new IdentityRole("Employee"));
                    }
                    if (!await roleManager.RoleExistsAsync("Manager"))
                    {
                        await roleManager.CreateAsync(new IdentityRole("Manager"));
                    }
                }
                // Configure the HTTP request pipeline.
                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Home/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
        }
    }
}
