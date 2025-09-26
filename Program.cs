using System.Globalization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Ams.Media.Web.Data;
using Ams.Media.Web.Services;

// ★ เพิ่ม using สำหรับ Options
using Ams.Media.Web.Options;

var builder = WebApplication.CreateBuilder(args);

// MVC (+ RuntimeCompilation เฉพาะตอน Dev)
var mvc = builder.Services.AddControllersWithViews();
if (builder.Environment.IsDevelopment())
{
    mvc.AddRazorRuntimeCompilation();
}

// EF Core SQL Server
builder.Services.AddDbContext<AmsDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("AmsDb")));

// Cookie Auth
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/Account/Login";
        o.AccessDeniedPath = "/Account/Denied";
        o.Cookie.Name = ".Ams.Media.Auth";
        o.ExpireTimeSpan = TimeSpan.FromHours(8);
        o.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

// Services กลาง
builder.Services.AddSingleton<IDateTimeHelper, DateTimeHelper>();
builder.Services.AddScoped<IMenuGate, MenuGate>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IQueryService, QueryService>();

// ★ Bind AmsOptions จาก appsettings:AMS
builder.Services.Configure<AmsOptions>(
    builder.Configuration.GetSection("AMS"));

var app = builder.Build();

// ใช้ Culture: en-US, th-TH (Default = en-US)
var supportedCultures = new[] { new CultureInfo("en-US"), new CultureInfo("th-TH") };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-US"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // ★ ต้องมาก่อน
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
