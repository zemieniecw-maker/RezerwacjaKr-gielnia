using RezerwacjaKrêgielnia.Data;
using RezerwacjaKrêgielnia.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Konfiguracja bazy danych SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<BowlingDbContext>(options =>
    options.UseSqlite(connectionString));

// 2. Konfiguracja Logowania
builder.Services.AddDefaultIdentity<UserEntity>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Nie wymagamy potwierdzania emailem
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
})
    .AddRoles<IdentityRole>() // Obsługa ról
    .AddEntityFrameworkStores<BowlingDbContext>();

// Rejestracja naszego serwisu
builder.Services.AddScoped<RezerwacjaKrêgielnia.Services.ReservationService>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    // Uruchomienie seedera
    await RezerwacjaKrêgielnia.Data.DbSeeder.SeedRolesAndAdminAsync(services);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization(); // To musi byæ!

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages(); // Potrzebne do stron logowania

app.Run();
