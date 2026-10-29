using GestionOrdinateurs.Shared;
using GestionOrdinateurs.API.Events;

namespace GestionOrdinateurs.API.Services;

public interface IOrdinateurService
{
	// Définit les opérations disponibles pour gérer les ordinateurs
	Task<List<Ordinateur>> GetOrdinateurs();
	Task<Ordinateur?> GetOrdinateurById(int id);
	Task AjouterOrdinateur(Ordinateur ordinateur);
	Task<bool> SupprimerOrdinateur(int id);
	Task<bool> OrdinateurModifie(int id, Ordinateur updatedOrdinateur);

	Task<List<Ordinateur>> GetOrdinateursDisponibles();
	Task<List<Ordinateur>> GetOrdinateursParMarque(string marque);
	Task<List<Ordinateur>> GetOrdinateursParPrix();
	string Diagnostiquer(Ordinateur ordinateur);
	(int TotalOrdinateur, int OrdiDisponibles, decimal ValeurStock, int NbReparations) CalculerStatistiques();

	event EventHandler<OrdinateurAjouteEventArgs>? OrdinateurAjoute;
	event EventHandler<OrdinateurSupprimeEventArgs>? OrdinateurSupprime;
	event EventHandler<OrdinateurMisAJourEventArgs>? OrdinateurMisAJour;
    
}
