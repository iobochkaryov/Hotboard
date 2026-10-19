using Hotboard.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
// require authentication globally for Razor Pages
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

// Redirect anonymous requests to login (except Identity area and static files)
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value ?? string.Empty;
    if (!context.User.Identity?.IsAuthenticated ?? true)
    {
        // Allow access to Identity UI (login/logout, static assets, css/js, favicon)
        if (!path.StartsWith("/Identity", StringComparison.OrdinalIgnoreCase)
            && !path.StartsWith("/lib", StringComparison.OrdinalIgnoreCase)
            && !path.StartsWith("/css", StringComparison.OrdinalIgnoreCase)
            && !path.StartsWith("/js", StringComparison.OrdinalIgnoreCase)
            && !path.StartsWith("/favicon.ico", StringComparison.OrdinalIgnoreCase)
            && !path.StartsWith("/Account/Login", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.Redirect("/Identity/Account/Login");
            return;
        }
    }

    await next();
});

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

// Seed roles and admin user
await SeedData.InitializeAsync(app.Services);

app.Run();
