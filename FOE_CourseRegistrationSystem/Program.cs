using FOE_CourseRegistrationSystem.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

//Database Connection Setup
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Test Database Connection BEFORE Application Starts
try
{
    using (var connection = new SqlConnection(connectionString))
    {
        connection.Open();
        Console.WriteLine("✅ Successfully connected to SQL Server!");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ SQL Connection Error: {ex.Message}");
    throw;
}

// ✅ Add services to the container BEFORE `builder.Build();`
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// ✅ Authentication & Authorization Setup (BEFORE `builder.Build();`)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("StudentOnly", policy => policy.RequireRole("Student"));
    options.AddPolicy("AdviserOnly", policy => policy.RequireRole("Adviser"));
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("AR"));
});

var app = builder.Build();

// ✅ Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// ✅ Ensure Authentication Middleware is Used BEFORE Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
