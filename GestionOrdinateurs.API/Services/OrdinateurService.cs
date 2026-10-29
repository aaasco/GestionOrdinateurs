using GestionOrdinateurs.Shared;
using GestionOrdinateurs.API.Events;
using Microsoft.EntityFrameworkCore;

namespace GestionOrdinateurs.API.Services;

//Service pour gérer les ordinateurs dans le parc
public class OrdinateurService : IOrdinateurService
{
    //Contexte de la base de données pour accéder aux ordinateurs
    private readonly ParcInformatiqueContext context;
    private readonly JournalService journalService;
    
    //Événements déclenchés lors de l'ajout, la mise à jour ou la suppression d'un ordinateur
    public event EventHandler<OrdinateurAjouteEventArgs>? OrdinateurAjoute;
    public event EventHandler<OrdinateurMisAJourEventArgs>? OrdinateurMisAJour;
    public event EventHandler<OrdinateurSupprimeEventArgs>? OrdinateurSupprime;


    //Récupère la liste des ordinateurs initialisés dans le parc
    public OrdinateurService(ParcInformatiqueContext context, JournalService journalService)
    {
        this.context = context;
        this.journalService = journalService;
    }

    //Retourne la liste des ordinateurs présents dans le parc
    public async Task<List<Ordinateur>> GetOrdinateurs()
    {
        return await context.Ordinateurs.ToListAsync();
    }

    //Recherche un ordinateur par son identifiant
    public async Task<Ordinateur?> GetOrdinateurById(int id)
    {
        return await context.Ordinateurs.FirstOrDefaultAsync(o => o.Id == id);
    }

    //Ajoute un nouvel ordinateur au parc
    public async Task AjouterOrdinateur(Ordinateur ordinateur)
    {
        //Ajoute l'ordinateur à la base de données et sauvegarde les changements
        context.Ordinateurs.Add(ordinateur);
        await context.SaveChangesAsync();

        //Enregistre l'ajout de l'ordinateur dans le journal
        journalService.Ecrire($"{DateTime.Now}: Ordinateur ajouté : {ordinateur.Nom}");

        //Déclenchement de l'événement d'ajout d'ordinateur
        OrdinateurAjoute?.Invoke(this, new OrdinateurAjouteEventArgs(ordinateur));
    }

    //Supprime un ordinateur du parc par son identifiant
    public async Task<bool> SupprimerOrdinateur(int id)
    {
        Ordinateur? ordinateur = await GetOrdinateurById(id);

        //Vérifie si l'ordinateur existe avant de le supprimer
        if (ordinateur == null)
        {
            return false;
        }

        context.Ordinateurs.Remove(ordinateur);
        await context.SaveChangesAsync();

        //Enregistre la suppression de l'ordinateur dans le journal
        journalService.Ecrire($"{DateTime.Now}: Ordinateur supprimé : {ordinateur.Nom}");

        //Déclenchement de l'événement de suppression d'ordinateur
        OrdinateurSupprime?.Invoke(this, new OrdinateurSupprimeEventArgs(ordinateur));

        return true;
    }

    //Met à jour les informations d'un ordinateur existant
    public async Task<bool> OrdinateurModifie(int id, Ordinateur updatedOrdinateur)
    {
        Ordinateur? ordinateur = await GetOrdinateurById(id);

        //Vérifie si l'ordinateur existe avant de le mettre à jour
        if (ordinateur == null)
        {
            return false;
        }

        ordinateur.Nom = updatedOrdinateur.Nom;
        ordinateur.Caracteristiques = updatedOrdinateur.Caracteristiques;
        ordinateur.ModifierEtat(updatedOrdinateur.Etat);
        ordinateur.Prix = updatedOrdinateur.Prix;
        ordinateur.Stock = updatedOrdinateur.Stock;

        await context.SaveChangesAsync();

        //Enregistre la mise à jour de l'ordinateur dans le journal
        journalService.Ecrire($"{DateTime.Now}: Ordinateur mis à jour : {ordinateur.Nom}");

        //Déclenchement de l'événement de mise à jour d'ordinateur
        OrdinateurMisAJour?.Invoke(this, new OrdinateurMisAJourEventArgs(ordinateur));

        return true;
    }

    // Retourne uniquement les ordinateurs ayant du stock disponible
    public async Task<List<Ordinateur>> GetOrdinateursDisponibles()
    {
        return await context.Ordinateurs.Where(o => o.Stock > 0).ToListAsync();
    }

    //Recherche les ordinateurs par marque
    public async Task<List<Ordinateur>> GetOrdinateursParMarque(string marque)
    {
        return await context.Ordinateurs.Where(o => o.Caracteristiques.Marque == marque).ToListAsync();
    }

    // Retourne les ordinateurs du plus cher au moins cher
    public async Task<List<Ordinateur>> GetOrdinateursParPrix()
    {
        var ordinateurs = await context.Ordinateurs.ToListAsync();
        return ordinateurs.OrderByDescending(o => o.Prix).ToList();
    }

    // Détermine le diagnostic d'un ordinateur en fonction de son état et de ses caractéristiques
    public string Diagnostiquer(Ordinateur ordinateur)
    {
        return ordinateur switch
        {
            { Etat: EtatOrdinateur.HorsService } => "Remplacement",
            { Etat: EtatOrdinateur.Lent, Caracteristiques: { RamGo: < 8 } } => "Ajouter RAM", _ => "Aucune action"
        };
    }

    // Calcule les statistiques du parc informatique
    public (int TotalOrdinateur, int OrdiDisponibles, decimal ValeurStock, int NbReparations) CalculerStatistiques()
    {
        //Récupère les ordinateurs de la BDD
        List<Ordinateur> ordinateurs = context.Ordinateurs.ToList();

        // Calcule les différentes statistiques
        int total = ordinateurs.Count;
        int disponibles = ordinateurs.Count(o => o.Stock > 0);
        decimal valeurStock = ordinateurs.Sum(o => o.Prix * o.Stock);
        int nbReparations = context.Reparations.Count();

        return (total, disponibles, valeurStock, nbReparations);
    }
}