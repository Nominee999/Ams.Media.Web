// Program.cs
using Ams.Media.Web.Data;
using Ams.Media.Web.Repositories;
using Ams.Media.Web.Repositories.Interfaces;
using Ams.Media.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Db (สำหรับบางส่วนที่อาจใช้ EF)
builder.Services.AddDbContext<AmsDbContext>(opt =>
{
    var cs = builder.Configuration.GetConnectionString("AmsDb")
          ?? builder.Configuration.GetConnectionString("Default");
    if (!string.IsNullOrWhiteSpace(cs))
        opt.UseSqlServer(cs);
});

// MVC / Razor
builder.Services.AddControllersWithViews();

// Auth (Cookie)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opt =>
    {
        opt.LoginPath = "/Account/Login";
        opt.LogoutPath = "/Account/Logout";
        opt.Cookie.Name = "Ams.Media.Auth";
    });

builder.Services.AddAntiforgery();

// DI: Repositories / Services (ตามที่ใช้งานจริง)
builder.Services.AddScoped<IClientAddressRepository, ClientAddressRepository>();
builder.Services.AddScoped<IClientService, ClientService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
