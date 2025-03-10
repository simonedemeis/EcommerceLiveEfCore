using EcommerceLiveEfCore.Data;
using EcommerceLiveEfCore.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//recupero la stringa di connessione
var connectionstring = builder.Configuration.GetConnectionString("DefaultConnection");

//iniettiamo il dbcontext nel container dei servizi
builder.Services.AddDbContext<EcommerceLiveEfCoreDbContext>(options =>
    //scelgo il provider di database da utilizzare (UseSqlServer, UseMySql, UsePostgreSql...)
    options.UseSqlServer(connectionstring)
);

builder.Services.AddScoped<ProductService>();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
