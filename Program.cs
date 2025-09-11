using Ams.Media.Web.Data;
using Ams.Media.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Database
builder.Services.AddDbContext<AmsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AmsDb")));

// 2. Dependency Injection
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMenuGate, MenuGate>();

// 3. Authentication & Cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";   // หน้า Login
        options.LogoutPath = "/Account/Logout"; // หน้า Logout
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
    });

// 4. Authorization
builder.Services.AddAuthorization();

// 5. MVC + Razor
builder.Services.AddControllersWithViews();

var app = builder.Build();

// -------------------- Pipeline --------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Auth middlewares (สำคัญมาก!)
app.UseAuthentication();
app.UseAuthorization();

// Default Route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Start app
app.Run();
