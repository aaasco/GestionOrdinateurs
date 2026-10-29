
using Microsoft.AspNetCore.Mvc;

namespace AuthentificationApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IConfiguration configuration;
    private readonly IHttpClientFactory httpClientFactory;

    public AuthenticationController(
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory)
    {
        this.configuration = configuration;
        this.httpClientFactory = httpClientFactory;
    }

    /*
    // Ancienne méthode de connexion sans Keycloak
    [HttpPost("login")]
    public async Task<IActionResult> Login(string login)
    {
        HttpClient client = httpClientFactory.CreateClient();

        // Recherche l'utilisateur dans l'API
        HttpResponseMessage response = await client.GetAsync(
            $"http://gestionordinateurs-api:5112/api/Users/{Uri.EscapeDataString(login)}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return NotFound("Cet utilisateur n'existe pas.");
        }

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode(502, "Erreur lors de la recherche de l'utilisateur.");
        }

        User? utilisateur = await response.Content.ReadFromJsonAsync<User>();

        if (utilisateur == null)
        {
            return NotFound("Cet utilisateur n'existe pas.");
        }

        string jwtKey = configuration["Jwt:Key"] ?? "";
        byte[] key = Encoding.ASCII.GetBytes(jwtKey);

        Claim[] claims =
        {
            new Claim(ClaimTypes.Name, utilisateur.Login),
            new Claim(ClaimTypes.Role, utilisateur.Role.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        string jwt = tokenHandler.WriteToken(token);

        return Ok(jwt);
    }
    */

    // Nouvelle méthode de connexion avec Keycloak
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        string baseUrl = configuration["Keycloak:BaseUrl"] ?? "";
        string realm = configuration["Keycloak:Realm"] ?? "";
        string clientId = configuration["Keycloak:ClientId"] ?? "";
        string clientSecret = configuration["Keycloak:ClientSecret"] ?? "";

        if (string.IsNullOrWhiteSpace(clientSecret))
        {
            return StatusCode(500, "Client secret Keycloak non configuré.");
        }

        // Adresse du service Keycloak qui délivre les JWT
        string url = $"{baseUrl}/realms/{realm}/protocol/openid-connect/token";

        // Informations envoyées à Keycloak
        var donnees = new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["client_id"] = clientId,
            ["client_secret"] = clientSecret,
            ["username"] = request.Username,
            ["password"] = request.Password
        };

        HttpClient client = httpClientFactory.CreateClient();

        // Demande du JWT à Keycloak
        HttpResponseMessage response = await client.PostAsync(
            url,
            new FormUrlEncodedContent(donnees));

        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest ||
                response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return Unauthorized(
                    "Identifiant ou mot de passe incorrect, ou compte non configuré.");
            }

            return StatusCode(502, "Erreur de communication avec Keycloak.");
        }

        // Renvoie la réponse de Keycloak contenant le JWT
        string resultat = await response.Content.ReadAsStringAsync();

        return Content(resultat, "application/json");
    }
}

// Données reçues depuis le formulaire de connexion
public class LoginRequest
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
}
