using System.Globalization;
using System.IO.Compression;
using Instagram.Controllers;
using Instagram.Database;
using Instagram.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services
    .AddDbContext<InstagramDbContext>(o => o.UseNpgsql(connectionString))
    .AddIdentity<User, IdentityRole>(o =>
    {
        o.Password.RequiredLength = 5;
        o.Password.RequireDigit = false;
        o.Password.RequireLowercase = false;
        o.Password.RequireNonAlphanumeric = false;
        o.Password.RequireUppercase = false;
    })
    .AddEntityFrameworkStores<InstagramDbContext>();

builder.Services.AddTransient<UserController>();
builder.Services.AddTransient<AccountController>();
builder.Services.AddMemoryCache();

builder.Services.AddResponseCompression(o =>
{
    o.EnableForHttps = true;
    o.Providers.Add<GzipCompressionProvider>();
    o.Providers.Add<BrotliCompressionProvider>();
});

builder.Services.Configure<GzipCompressionProviderOptions>(o =>
{
    o.Level = CompressionLevel.Optimal;
});

builder.Services.Configure<BrotliCompressionProviderOptions>(o =>
{
    o.Level = CompressionLevel.Optimal;
});

builder.Services.AddLocalization(o => o.ResourcesPath = "Resources");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

var supportedCultures = new[]
{
    new CultureInfo("en"),
    new CultureInfo("ru")
};

app.UseRequestLocalization(new RequestLocalizationOptions()
{
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures,
    DefaultRequestCulture = new RequestCulture("ru")
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Index}/{id?}");

app.UseResponseCompression();

app.Run();