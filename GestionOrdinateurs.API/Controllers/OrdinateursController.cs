using GestionOrdinateurs.API.Services;
using GestionOrdinateurs.Shared;
using GestionOrdinateurs.API.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GestionOrdinateurs.API.Controllers;

// Controller pour gérer les ordinateurs dans le parc
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrdinateursController : ControllerBase
{
private readonly IOrdinateurService ordinateurService;
private readonly ILogger<OrdinateursController> logger;
private readonly IConfiguration configuration;
private readonly ParcInformatiqueContext context;

// Initialise le service utilisé par le controller
public OrdinateursController(IOrdinateurService ordinateurService, ILogger<OrdinateursController> logger, IConfiguration configuration, ParcInformatiqueContext context)
    {
        this.ordinateurService = ordinateurService;
        this.logger = logger;
        this.configuration = configuration;
        this.context = context;

        ordinateurService.OrdinateurAjoute += AfficherMessageAjout;
        ordinateurService.OrdinateurSupprime += AfficherMessageSuppression;
        ordinateurService.OrdinateurMisAJour += AfficherMessageMiseAJour;
    }

    private void AfficherMessageAjout(object? sender, OrdinateurAjouteEventArgs e)
    {
        Console.WriteLine($"Ordinateur ajouté : {e.Ordinateur.Nom}");
    }
    private void AfficherMessageSuppression(object? sender, OrdinateurSupprimeEventArgs e)
    {
        Console.WriteLine($"Ordinateur supprimé : {e.Ordinateur.Nom}");
    }
    private void AfficherMessageMiseAJour(object? sender, OrdinateurMisAJourEventArgs e)
    {
        Console.WriteLine($"Ordinateur mis à jour : {e.Ordinateur.Nom}");
    }

    //Retourne les informations de l'application depuis la configuration
    [HttpGet("info")]
    public IActionResult RecupererInfoApplication()
    {
        logger.LogInformation("Récupération des informations de l'application depuis la configuration");

        string? nomApplication = configuration["Application:Name"];
        string? versionApplication = configuration["Application:Version"];
        return Ok(new { NomApplication = nomApplication, VersionApplication = versionApplication });
    }

    //Retourne la liste des ordinateurs présents dans le parc
    [HttpGet]
    public async Task<ActionResult<List<Ordinateur>>> GetOrdinateurs()
    {
        logger.LogInformation("Récupération de la liste des ordinateurs");
        return Ok(await ordinateurService.GetOrdinateurs());
    }

    //Retourne un ordinateur par son identifiant
    [HttpGet("{id}")]
    public async Task<ActionResult<Ordinateur>> GetOrdinateurById(int id)
    {

        logger.LogInformation($"Récupération de l'ordinateur avec l'ID {id}");
        Ordinateur? ordinateur = await ordinateurService.GetOrdinateurById(id);
        if (ordinateur == null)
        {
            return NotFound();
        }

        return Ok(ordinateur);
    }

    // Retourne le diagnostic d'un ordinateur par son identifiant
    [HttpGet("{id}/diagnostic")]
    public async Task<IActionResult> DiagnostiquerOrdinateur(int id)
    {
        Ordinateur? ordinateur = await ordinateurService.GetOrdinateurById(id);
        // Vérifie si l'ordinateur existe
        if (ordinateur == null)
        {
            return NotFound();
        }

        // Récupère le diagnostic de l'ordinateur
        string diagnostic = ordinateurService.Diagnostiquer(ordinateur);
        
        return Ok(new { Diagnostic = diagnostic });
    }

    //Ajoute un nouvel ordinateur au parc
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<Ordinateur>> AjouterOrdinateur(Ordinateur ordinateur)
    {
        logger.LogInformation($"Ajout d'un nouvel ordinateur avec l'ID {ordinateur.Id}");
        await ordinateurService.AjouterOrdinateur(ordinateur);
        return CreatedAtAction(nameof(GetOrdinateurById), new { id = ordinateur.Id }, ordinateur);
    }

    //Supprime un ordinateur du parc par son identifiant
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> SupprimerOrdinateur(int id)
    {
        logger.LogInformation($"Suppression de l'ordinateur avec l'ID {id}");
        bool supprime = await ordinateurService.SupprimerOrdinateur(id);
        if (!supprime)
        {
            return NotFound();
        }

        return NoContent();
    }

    //Met à jour les informations d'un ordinateur existant
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> OrdinateurModifie(int id, Ordinateur ordinateur)
    {
        logger.LogInformation($"Mise à jour de l'ordinateur avec l'ID {id}");
        bool misAJour = await ordinateurService.OrdinateurModifie(id, ordinateur);
        if (!misAJour)
        {
            return NotFound();
        }

        return Ok();
    }

    // Affiche les ordinateurs disponibles en stock
    [HttpGet("disponibles")]
    public async Task<ActionResult<List<Ordinateur>>> GetOrdinateursDisponibles()
    {
        logger.LogInformation("Récupération de la liste des ordinateurs disponibles");
        return Ok(await ordinateurService.GetOrdinateursDisponibles());
    }

    // Recherche les ordinateurs par marque
    [HttpGet("marque/{marque}")]
    public async Task<ActionResult<List<Ordinateur>>> GetOrdinateursParMarque(string marque)
    {
        logger.LogInformation($"Récupération de la liste des ordinateurs de la marque {marque}");
        return Ok(await ordinateurService.GetOrdinateursParMarque(marque));
    }

    // Retourne les ordinateurs du plus cher au moins cher
    [HttpGet("prix")]
    public async Task<ActionResult<List<Ordinateur>>> GetOrdinateursParPrix()
    {
        logger.LogInformation("Récupération de la liste des ordinateurs par prix décroissant");
        return Ok(await ordinateurService.GetOrdinateursParPrix());
    }
    
    // Retourne les statistiques du parc informatique
    [HttpGet("statistiques")]
    public IActionResult GetStatistiques()
    {
        logger.LogInformation("Récupération des statistiques du parc informatique");
        // Récupère et décompose les statistiques du parc informatique
        var (total, disponibles, valeurStock, nbReparations) = ordinateurService.CalculerStatistiques();
        
        return Ok(new
        {
            TotalOrdinateur = total,
            OrdiDisponibles = disponibles,
            ValeurStock = valeurStock,
            NbReparations = nbReparations
        });
    }

    // Recherche un ordinateur dans l'inventaire par son nom
    [HttpGet("inventaire/{nom}")]
    public IActionResult GetOrdinateurInventaire(string nom)
    {
        logger.LogInformation($"Recherche de l'ordinateur dans l'inventaire avec le nom {nom}");
        // Crée un inventaire d'ordinateurs à partir de la base de données
        InventaireOrdinateurs inventaire = new InventaireOrdinateurs();
        inventaire.Ordinateurs = context.Ordinateurs.ToList();

        // Recherche l'ordinateur grâce à l'indexeur
        Ordinateur? ordinateur = inventaire[nom];
        
        if (ordinateur == null)
        {
            return NotFound();
        }

        return Ok(ordinateur);
    }
}