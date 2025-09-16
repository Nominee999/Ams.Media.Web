using System.Globalization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Ams.Media.Web.Data;
using Ams.Media.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// MVC + RuntimeCompilation (สะดวกช่วง Dev)
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

// EF Core SQL Server
builder.Services.AddDbContext<AmsDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("AmsDb")));




// Cookie Auth
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Denied";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        // กัน 400 จาก cookie size ใหญ่ผิดปกติ
        options.Cookie.Name = ".Ams.Media.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

// Localization (EN/TH)
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Services กลาง
builder.Services.AddSingleton<IDateTimeHelper, DateTimeHelper>();
builder.Services.AddScoped<IMenuGate, MenuGate>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IQueryService, QueryService>();
builder.Services.AddControllersWithViews();
builder.Services.AddAuthorization();


var app = builder.Build();

// ใช้ Culture: en-US, th-TH (Default = en-US) + CE Calendar
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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
