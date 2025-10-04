// Program.cs — AMS Media (NET 8)

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ================= MVC =================
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

// ================= DbContext =================
var connStr =
    builder.Configuration.GetConnectionString("AmsDb")
    ?? builder.Configuration.GetConnectionString("Default")
    ?? "";

builder.Services.AddDbContext<Ams.Media.Web.Data.AmsDbContext>(opt =>
{
    opt.UseSqlServer(connStr);
});

// ================= Dependency Injection =================
// NOTE: ใช้ namespace เดียวกับที่ใช้อยู่จริงในโปรเจกต์ตอนนี้
builder.Services.AddScoped<
    Ams.Media.Web.Repositories.IClientRepository,
    Ams.Media.Web.Repositories.ClientRepository>();

builder.Services.AddScoped<
    Ams.Media.Web.Services.IClientService,
    Ams.Media.Web.Services.ClientService>();

builder.Services.AddScoped<
    Ams.Media.Web.Services.IAuthService,
    Ams.Media.Web.Services.AuthService>();

builder.Services.AddScoped<
    Ams.Media.Web.Services.IMenuGate,
    Ams.Media.Web.Services.MenuGate>();

// ================= Authentication (Cookie) =================
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/Account/Login";
        o.AccessDeniedPath = "/Denied";
        o.Cookie.Name = "Ams.Media.Auth";
    });

// ================= Build app =================
var app = builder.Build();

// ================= Pipeline =================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// ================= Routes =================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
