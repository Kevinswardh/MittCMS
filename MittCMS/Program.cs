using Azure.Identity;
using Azure.Extensions.AspNetCore.Configuration.Secrets;


var builder = WebApplication.CreateBuilder(args);

// === Lägg till Azure Key Vault ===
// Detta gör att appen hämtar hemligheter (t.ex. umbracoDbDSN) direkt från ditt Key Vault
builder.Configuration.AddAzureKeyVault(
    new Uri("https://mittcms-keys.vault.azure.net/"),
    new DefaultAzureCredential());

// === Skapa Umbraco ===
builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddComposers()
    .Build();

var app = builder.Build();

// === Starta Umbraco ===
await app.BootUmbracoAsync();

// === Middleware ===
app.UseHttpsRedirection();

app.UseUmbraco()
    .WithMiddleware(u =>
    {
        u.UseBackOffice();
        u.UseWebsite();
    })
    .WithEndpoints(u =>
    {
        u.UseBackOfficeEndpoints();
        u.UseWebsiteEndpoints();
    });

await app.RunAsync();
