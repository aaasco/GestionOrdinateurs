
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GestionOrdinateurs.Client;
using Blazored.SessionStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

// Permet de sauvegarder le token JWT dans la session du navigateur
builder.Services.AddBlazoredSessionStorage();

// Configure un client HTTP pour appeler l'API d'authentification
builder.Services.AddHttpClient("AuthentificationApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:5157");
});

// Configure un client HTTP pour appeler l'API de gestion des ordinateurs
builder.Services.AddHttpClient("GestionOrdinateursApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:5010");
});

await builder.Build().RunAsync();
