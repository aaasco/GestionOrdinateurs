using GestionOrdinateurs.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace GestionOrdinateurs.API.Controllers;

// Controller permettant de consulter les utilisateurs
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ParcInformatiqueContext context;

    // Initialise le contexte de la base de données
    public UsersController(ParcInformatiqueContext context)
    {
        this.context = context;
    }

    // Récupère tous les utilisateurs
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        List<User> utilisateurs = await context.Users.ToListAsync();

        return Ok(utilisateurs);
    }

    // Recherche un utilisateur par son login
    [HttpGet("{login}")]
    public async Task<IActionResult> GetUserByLogin(string login)
    {
        User? utilisateur = await context.Users.FirstOrDefaultAsync(u => u.Login == login);

        if (utilisateur == null)
        {
            return NotFound("Cet utilisateur n'existe pas.");
        }

        return Ok(utilisateur);
    }
}