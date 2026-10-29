using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GestionOrdinateurs.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AuthentificationApi.Controllers;

// Controller permettant de générer un token JWT
[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IConfiguration configuration;
    private readonly IHttpClientFactory httpClientFactory;

    // Injection de la configuration et de HttpClientFactory
    public AuthenticationController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        this.configuration = configuration;
        this.httpClientFactory = httpClientFactory;
    }

    // Génère un token JWT à partir du login
    [HttpPost("login")]
    public async Task<IActionResult> Login(string login)
    {

        // Crée un client HTTP grâce à HttpClientFactory
        HttpClient client = httpClientFactory.CreateClient();

        // Appelle l'API pour rechercher l'utilisateur par son login
        HttpResponseMessage response = await client.GetAsync($"http://gestionordinateurs-api:5112/api/Users/{Uri.EscapeDataString(login)}");

        // Vérifie si l'utilisateur existe
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return NotFound("Cet utilisateur n'existe pas.");
        }

        // Vérifie si l'appel à l'API a réussi
        if (!response.IsSuccessStatusCode)
        {
            return StatusCode(502, "Erreur lors de la recherche de l'utilisateur.");
        }

        // Récupère les informations de l'utilisateur depuis la réponse de l'API
        User? utilisateur = await response.Content.ReadFromJsonAsync<User>();

        if (utilisateur == null)
        {
            return NotFound("Cet utilisateur n'existe pas.");
        }

        // Récupère la clé JWT dans appsettings.json
        string jwtKey = configuration["Jwt:Key"] ?? "";
        byte[] key = Encoding.ASCII.GetBytes(jwtKey);

        // Informations enregistrées dans le token
        Claim[] claims =
        {
            new Claim(ClaimTypes.Name, utilisateur.Login),
            new Claim(ClaimTypes.Role, utilisateur.Role.ToString())
        };

        // Configuration du token JWT
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        // Création et conversion du token en chaîne de caractères
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        string jwt = tokenHandler.WriteToken(token);

        return Ok(jwt);
    }
}