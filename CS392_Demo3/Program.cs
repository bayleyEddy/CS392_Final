using CS392_Demo3.Data;
using CS392_Demo3.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Razor Pages
builder.Services.AddRazorPages();

// Enable session (required for chatbot history)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// SQL Server + Identity
var conn = builder.Configuration.GetConnectionString("HOST")
           ?? throw new InvalidOperationException("Connection string 'HOST' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(conn));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultUI()
.AddDefaultTokenProviders();

// MongoDB + AI Services
builder.Services.AddSingleton<MongoDBService>();
builder.Services.AddHttpClient<AIService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Authentication + Authorization
app.UseAuthentication();
app.UseAuthorization();

// Session MUST be after routing and before Razor Pages
app.UseSession();

app.MapRazorPages();

app.Run();
