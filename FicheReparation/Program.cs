using FicheReparation.Data;
using FicheReparation.Entity;
using Microsoft.EntityFrameworkCore;
using DinkToPdf;
using DinkToPdf.Contracts;
using FicheReparation.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

//Client
builder.Services.AddScoped<IClientRepository, ClientRepository>();

//DemandeReparation
builder.Services.AddScoped<IDemandeReparationRepository, DemandeReparationRepository>();

// Configuration de DinkToPdf.
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
builder.Services.AddSingleton<PdfService>();

//Email service
builder.Services.AddScoped<IEmailService, EmailService>();

//le service HttpClient
builder.Services.AddHttpClient();


// Récupère la chaîne de connexion définie dans le fichier appsettings.json.
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Ajout d'Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => {
    // Options de configuration
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    // Autres options...
})
 .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI(); // Ajoute les pages Razor d'Identity par défaut


// Add services to the container.
builder.Services.AddControllersWithViews();

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

app.UseAuthentication();  // À placer AVANT UseAuthorization
app.UseAuthorization();

app.MapRazorPages(); // Ajoute cette ligne pour utiliser Identity UI



app.Run();
