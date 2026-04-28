using CS392_Demo3.Data;
using CS392_Demo3.Services;   // <-- Add this so Program.cs sees MongoDBService
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var conn = builder.Configuration.GetConnectionString("HOST")
           ?? throw new InvalidOperationException("Connection string 'HOST' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(conn));

builder.Services.AddDbContext<SchoolDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SchoolDbContext")
        ?? builder.Configuration.GetConnectionString("ApplicationDbContext")
        ?? throw new InvalidOperationException("No suitable connection string found for SchoolDbContext.")));

// Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
       .AddEntityFrameworkStores<ApplicationDbContext>()
       .AddDefaultUI()
       .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<MongoDBService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    await DbSeeder.SeedRolesAndAdminAsync(scope.ServiceProvider);
}

app.Run();
