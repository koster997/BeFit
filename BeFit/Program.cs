using BeFit.Data;
using BeFit.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//
// === KONFIGURACJA BAZY DANYCH ===
//
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//
// === IDENTITY + ROLE ===
//
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 4;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddSingleton<IEmailSender, DummyEmailSender>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

var app = builder.Build();

//
// === ŚRODOWISKO ===
//
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

//
// === PIPELINE ===
//
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

//
// === UTWORZENIE KONTA ADMINISTRATORA ===
//
await CreateAdminAccountAsync(app.Services);

app.Run();

//
// === METODA SEEDUJĄCA ADMINA ===
//
static async Task CreateAdminAccountAsync(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    const string adminRole = "Administrator";
    const string adminEmail = "admin@befit.pl";
    const string adminPassword = "Admin123!";

    // 1. Utwórz rolę Administrator, jeśli jeszcze jej nie ma
    if (!await roleManager.RoleExistsAsync(adminRole))
    {
        await roleManager.CreateAsync(new IdentityRole(adminRole));
        Console.WriteLine($"✔ Utworzono rolę: {adminRole}");
    }

    // 2. Utwórz użytkownika, jeśli nie istnieje
    var admin = await userManager.FindByEmailAsync(adminEmail);
    if (admin == null)
    {
        admin = new ApplicationUser
        {
            UserName = "admin",
            Email = adminEmail,
            EmailConfirmed = true,
            FirstName = "System",
            LastName = "Administrator",
            BirthDate = DateTime.Now.AddYears(-30)
        };

        var result = await userManager.CreateAsync(admin, adminPassword);
        if (!result.Succeeded)
        {
            Console.WriteLine("❌ Nie udało się utworzyć administratora:");
            foreach (var e in result.Errors)
                Console.WriteLine($"   - {e.Description}");
            return;
        }
    }

    // 3. Upewnij się, że admin ma przypisaną rolę Administrator
    if (!await userManager.IsInRoleAsync(admin, adminRole))
    {
        await userManager.AddToRoleAsync(admin, adminRole);
        Console.WriteLine("✔ Administratorowi przypisano rolę");
    }
    else
    {
        Console.WriteLine("ℹ Administrator już posiada właściwą rolę.");
    }
}

//
// === FAKE EMAIL SENDER ===
//
public class DummyEmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        Console.WriteLine($"[EMAIL] {subject} -> {email}");
        return Task.CompletedTask;
    }
}