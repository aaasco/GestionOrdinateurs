using GestionOrdinateurs.Shared;
using Microsoft.EntityFrameworkCore;
using GestionOrdinateurs.API.Services;
using GestionOrdinateurs.API;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// Configuration de Swagger pour l'authentification JWT
builder.Services.AddSwaggerGen(options =>
{
    // Ajoute le bouton Authorize dans Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Saisir le token JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    // Permet à Swagger d'envoyer le JWT avec les requêtes
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});
builder.Services.AddScoped<IOrdinateurService, OrdinateurService>();
builder.Services.AddScoped<ReparationService>();
builder.Services.AddDbContext<ParcInformatiqueContext>();
builder.Services.AddScoped<StockService>();
builder.Services.AddScoped<JournalService>();


// Configuration de l'authentification JWT avec Keycloak
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Récupère la configuration de Keycloak via le réseau Docker
        options.MetadataAddress =
            "http://keycloak:8080/realms/GestionOrdinateurs/.well-known/openid-configuration";

        // Autorise HTTP pour notre environnement Docker local
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Vérifie que le JWT provient bien de notre Realm Keycloak
            ValidateIssuer = true,
            ValidIssuer = "http://localhost:8180/realms/GestionOrdinateurs",

            // Le JWT actuel contient "aud": "account"
            // La validation de l'audience sera configurée séparément
            ValidateAudience = false,

            // Vérifie la durée de validité et la signature du JWT
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            // Utilise notre mapper Keycloak pour récupérer les rôles
            RoleClaimType = "roles"
        };
    });


// Autorise les requêtes provenant du client Blazor
builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorClient", policy =>
    {
        policy.WithOrigins("http://localhost:5000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Initialisation de la base de données
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ParcInformatiqueContext>();

    // Applique les migrations de la base de données
    db.Database.Migrate();

    // Ajoute les utilisateurs uniquement si la table est vide
    if (!db.Users.Any())
    {
        db.Users.AddRange(
            new User { Login = "user1", Role = RoleEnum.Admin },
            new User { Login = "user2", Role = RoleEnum.Gestionnaire },
            new User { Login = "user3", Role = RoleEnum.Gestionnaire }
        );

        db.SaveChanges();
    }
}

// Middleware
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("BlazorClient");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseHttpsRedirection();

app.Run();