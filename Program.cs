using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WatchStoreApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Cliente invece di IdentityUser perchè è stata creata una classe personalizzata
builder.Services.AddDefaultIdentity<Cliente>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()// !!! PER I RUOLI
    .AddEntityFrameworkStores<ApplicationDbContext>();
    // .AddDefaultTokenProviders(); //!!! PER RESETS E CONFIRMATIONS

/*  !!! PER DIVERSI CHECK
builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = true;
});
*/

builder.Services.AddControllersWithViews();

var app = builder.Build();

/*  !!! PER I RUOLI
public static class ApplicationDbInitializer
{
    public static async Task EnsureRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        var roles = new List<string> { "Admin", "User" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
    
}
*/

/*  !!! PER GENERARE IN AUTOMATICO UN ADMIN
async Task SeedAdminUser(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
{
    if(!await roleManager.RoleExistsAsync("Admin2")) // Crea il ruolo Admin se non esiste
    {
        await roleManager.CreateAsync(new IdentityRole("Admin2"));
    }
    // Crea l'utente Admin se non esiste
    if(await userManager.FindByEmailAsync("info2@admin.com") == null)
    {
        var user = new IdentityUser
        {
            UserName = "info2@admin.com",
            Email = "info2@admin.com",
            EmailConfirmed = true,
        };

        var result = await userManager.CreateAsync(user, "Admin123!");  // Imposta la password dell'utente admin
        if(result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "Admin");    // Aggiungi l'utente admin al ruolo Admin
        }
    }
}
*/


/*  !!! GESTIONE DEI RUOLI
// Ambito di servizio di gestione ruoli
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        // Risolvi il RoleManager dal provider di servizi
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // Chiamata al metodo per assicurare che i ruoli esistano
        await ApplicationDbInitializer.EnsureRolesAsync(roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Un errore è avvenuto durante la creazione dei ruoli");
    }
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await SeedAdminUser(userManager, roleManager);
}

*/

/* OPPURE
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // Ensure roles exist
    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await ApplicationDbInitializer.EnsureRolesAsync(roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while creating roles");
    }
    
    // Seed admin user
    try
    {
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await SeedAdminUser(userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the admin user");
    }
}
*/ 


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); //!!! PER ACCESSO A DETERMINATE PAGINE
app.UseAuthorization();

// app.UseStatusCodePagesWithReExecute("/Home/Error"); //!!! 
// app.UseEndpoints(endpoints =>
// {
// 	endpoints.MapControllerRoute(
// 		name: "user",
// 		pattern: "User/{email}",
// 		defaults: new { controller = "Users", action = "Index" });
// });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// Seeding del database
using (var scope = app.Services.CreateScope())
{
	var serviceProvider = scope.ServiceProvider;
	try
	{
		var userManager = serviceProvider.GetRequiredService<UserManager<Cliente>>();
		var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
		await SeedData.InitializeAsync(userManager, roleManager);
	}
	catch (Exception ex)
	{
		var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
		logger.LogError(ex, "Un errore è avvenuto durante il seeding del database.");
	}
}

app.Run();