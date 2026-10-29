namespace GestionOrdinateurs.Shared;

// Record représentant une entrée de l'historique des réparations
public record HistoriqueReparationRecord(
    int OrdinateurId,
    string NomOrdinateur,
    string Technicien,
    string TypeReparation,
    DateTime DateIntervention
);