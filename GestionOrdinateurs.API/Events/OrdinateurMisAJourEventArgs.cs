using GestionOrdinateurs.Shared;
namespace GestionOrdinateurs.API.Events;

//Informations transmises lors de la mise à jour d'un ordinateur
public class OrdinateurMisAJourEventArgs : EventArgs
{
    public Ordinateur Ordinateur { get; }

    public OrdinateurMisAJourEventArgs(Ordinateur ordinateur)
    {
        Ordinateur = ordinateur;
    }
}