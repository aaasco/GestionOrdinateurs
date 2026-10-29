
using Microsoft.AspNetCore.Mvc;
using GestionOrdinateurs.API.Services;
using GestionOrdinateurs.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace GestionOrdinateurs.API.Controllers;

// Controller pour gérer les réparations d'ordinateurs
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReparationController : ControllerBase
{
    private readonly ReparationService reparationService;
    private readonly IOrdinateurService ordinateurService;
    private readonly ParcInformatiqueContext context;

    // Initialise le service utilisé par le controller 
    public ReparationController(
        ReparationService reparationService,
        IOrdinateurService ordinateurService,
        ParcInformatiqueContext context)
    {
        this.reparationService = reparationService;
        this.ordinateurService = ordinateurService;
        this.context = context;
    }

    // Simule la réparation de cinq ordinateurs
    [Authorize(Roles = "Admin")]
    [HttpPost("SimulerReparations")]
    public async Task<IActionResult> SimulerReparation()
    {
        // Récupère la liste des ordinateurs
        List<Ordinateur> listeOrdinateurs = await ordinateurService.GetOrdinateurs();

        // Vérifie qu'il y a au moins cinq ordinateurs
        if (listeOrdinateurs.Count < 5)
        {
            return BadRequest("Il n'y a pas assez d'ordinateurs pour simuler trois réparations.");
        }

        // Simule la réparation des cinq ordinateurs
        await Task.WhenAll(
            reparationService.SimulerReparationAsync(listeOrdinateurs[0]),
            reparationService.SimulerReparationAsync(listeOrdinateurs[1]),
            reparationService.SimulerReparationAsync(listeOrdinateurs[2]),
            reparationService.SimulerReparationAsync(listeOrdinateurs[3]),
            reparationService.SimulerReparationAsync(listeOrdinateurs[4])
        );

        return Ok("Les cinq réparations sont terminées.");
    }

    // Retourne toutes les réparations avec les ordinateurs associés
    [HttpGet]
    public async Task<IActionResult> GetReparations()
    {
        var reparations = await context.Reparations
            .Include(r => r.UnOrdinateur)
            .ToListAsync();

        return Ok(reparations);
    }

    // Retourne l'historique des réparations
    [HttpGet("/api/Reparations/historique")]
    public async Task<IActionResult> GetHistoriqueReparations()
    {
        // Récupère l'historique depuis le service de réparation
        List<HistoriqueReparationRecord> historique = await reparationService.GetHistoriqueReparations();
        return Ok(historique);
    }
}
