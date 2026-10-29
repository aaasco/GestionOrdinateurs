using GestionOrdinateurs.Shared;
using Microsoft.EntityFrameworkCore;

namespace GestionOrdinateurs.API.Services;

//Service de gestion des réparations d'ordinateurs
public class ReparationService
{
    private readonly ParcInformatiqueContext context;
    private readonly JournalService journalService;
    private readonly SemaphoreSlim verrou = new(1, 1); // Sémaphore pour gérer l'accès concurrent aux réparations

    public ReparationService(ParcInformatiqueContext context, JournalService journalService)
    {
        this.context = context;
        this.journalService = journalService;
    }

    //Simule une réparation d'un ordinateur
    public async Task SimulerReparationAsync(Ordinateur ordinateur)
    {
        Reparation reparation = new Reparation
        {
            UnOrdinateur = ordinateur,
            TypeReparation = "Réparation simulée",
            DateDebut = DateTime.Now,
            Statut = "En cours",
            Technicien = "Technicien 1"
        };

        // Protège l'ajout de la réparation dans la base
        await verrou.WaitAsync();
        try
        {
            context.Reparations.Add(reparation);
            await context.SaveChangesAsync();
        }
        finally
        {
            verrou.Release();
        }

        // Simule une réparation pendant 2 secondes
        await Task.Delay(2000);

        // Protège la mise à jour du statut
        await verrou.WaitAsync();
        try
        {
            reparation.Statut = "Terminée";
            await context.SaveChangesAsync();

            // Enregistre la réparation terminée dans le journal
            journalService.Ecrire($"{DateTime.Now}: Réparation terminée pour l'ordinateur {ordinateur.Nom}");
        }
        finally
        {
            verrou.Release();
        }
    }

    // Récupère l'historique des réparations pour un ordinateur donné
    public async Task<List<HistoriqueReparationRecord>> GetHistoriqueReparations()
    {
        return await context.Reparations
            .Select(r => new HistoriqueReparationRecord(
                r.UnOrdinateur.Id,
                r.UnOrdinateur.Nom,
                r.Technicien,
                r.TypeReparation,
                r.DateDebut
            ))
            .ToListAsync();
    }
}