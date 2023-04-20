using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Text;
using Zlagoda.Business.Interfaces;
using Zlagoda.Business.Repositories;
using Zlagoda.Models;
using Zlagoda.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Default");

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<ICategoryRepository, CategoryRepository>(_ => new CategoryRepository(connectionString));
builder.Services.AddSingleton<IProductRepository, ProductRepository>(_ => new ProductRepository(connectionString));
builder.Services.AddSingleton<IStoreProductRepository, StoreProductRepository>(_ => new StoreProductRepository(connectionString));
builder.Services.AddSingleton<IEmployeeRepository, EmployeeRepository>(_ => new EmployeeRepository(connectionString));
builder.Services.AddSingleton<ISaleRepository, SaleRepository>(_ => new SaleRepository(connectionString));
builder.Services.AddSingleton<ICustomerCardRepository, CustomerCardRepository>(_ => new CustomerCardRepository(connectionString));
builder.Services.AddSingleton<ICheckRepository, CheckRepository>(_ => new CheckRepository(connectionString));

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddCors(builder => {
    builder.AddPolicy("DefaultPolicy", option =>
    {
        option.AllowAnyMethod();
        option.AllowAnyOrigin();
        option.AllowAnyHeader();
    });
});

JwtTokenService.Configuration = builder.Configuration;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration[$"Jwt:issuer"],
        ValidAudience = builder.Configuration[$"Jwt:audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration[$"Jwt:secret"])),
        ClockSkew = TimeSpan.Zero
    };
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

var defaultDateCulture = "en-US";
var cultureInfo = new CultureInfo(defaultDateCulture);
cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
cultureInfo.NumberFormat.CurrencyDecimalSeparator = ".";

// Configure the Localization middleware
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(cultureInfo),
    SupportedCultures = new List<CultureInfo>
    {
        cultureInfo,
    },
    SupportedUICultures = new List<CultureInfo>
    {
        cultureInfo,
    }
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
