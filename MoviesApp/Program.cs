using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Movies.Services;
using MoviesApp.DataAccess;
using MoviesApp.Entities;
using MoviesApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connStr = builder.Configuration.GetConnectionString("MoviesDB");
builder.Services.AddDbContext<MovieDbContext>(options => options.UseSqlServer(connStr));

// with this configuration the mgr objects: User, Role, and Signin Managers become available in the DI container
// i.e. like the DB context object we can "inject" them into our controllers
// we also configure some password constraints
builder.Services.AddIdentity<User, IdentityRole>(options => {
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireDigit = true;
}).AddEntityFrameworkStores<MovieDbContext>().AddDefaultTokenProviders();

// add our movie manager svc:
builder.Services.AddScoped<IMovieManager, MovieManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using (var scope = scopeFactory.CreateScope())
{
    await MovieDbContext.CreateAdminUser(scope.ServiceProvider);
}

app.Run();
