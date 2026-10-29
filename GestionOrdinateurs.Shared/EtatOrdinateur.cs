namespace GestionOrdinateurs.Shared;

//Liste des états possibles pour un ordinateur
public enum EtatOrdinateur
{
    EnService,
    EnPanne,
    EnMaintenance,
    HorsService,
    Lent,
    Reparable
}