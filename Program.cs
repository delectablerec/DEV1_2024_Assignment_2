using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
//using DEV1_2024_Assignment_2.Data;
//using DEV1_2024_Assignment_2.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<Cliente>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Register your CartService here
builder.Services.AddScoped<CarrelloService>();  // Adding CartService as Scoped service

builder.Services.AddControllersWithViews();


/*  DA IMPLEMENTARE INSIEME ALLE SESSIONI 
// Configure the authentication cookie to initialize the cart on sign-in
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnSignedIn = async context =>
    {
        var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<Cliente>>();
        var carrelloService = context.HttpContext.RequestServices.GetRequiredService<CarrelloService>();

        var user = await userManager.GetUserAsync(context.Principal);
        if (user != null)
        {
            // Initialize the user's cart upon login if it's not already initialized
            carrelloService.InizializzaCarrello(user.Id);
            Console.WriteLine("Inizializzato Carrello da program");
        }
    };
});
*/


var app = builder.Build();

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
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
	endpoints.MapControllerRoute(
		name: "user",
		pattern: "User/{email}",
		defaults: new { controller = "Users", action = "Index" });
});

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