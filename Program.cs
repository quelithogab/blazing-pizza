using BlazingPizza.Data;
using BlazingPizza.Services;
using System.Globalization;

var cultureInfo = new CultureInfo("fr-HT");
cultureInfo.NumberFormat.CurrencySymbol = "HTG ";
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient();
builder.Services.AddSqlite<PizzaStoreContext>("Data Source=pizza.db");
builder.Services.AddScoped<OrderState>();

var app = builder.Build();

var localisationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("fr-HT")
    .AddSupportedCultures("fr-HT")
    .AddSupportedUICultures("fr-HT");
localisationOptions.SupportedCultures[0] = cultureInfo;
localisationOptions.SupportedUICultures[0] = cultureInfo;

app.UseRequestLocalization(localisationOptions);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");


// Initialize the database
var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using (var scope = scopeFactory.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PizzaStoreContext>();
    if (db.Database.EnsureCreated())
    {
        SeedData.Initialize(db);
    }
}

app.Run();