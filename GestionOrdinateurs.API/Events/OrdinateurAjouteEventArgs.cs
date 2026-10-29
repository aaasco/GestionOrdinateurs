using GestionOrdinateurs.Shared;

namespace GestionOrdinateurs.API.Events;

//Informations transmises lors de l'ajout d'un ordinateur
public class OrdinateurAjouteEventArgs : EventArgs
{
    public Ordinateur Ordinateur { get; }

    public OrdinateurAjouteEventArgs(Ordinateur ordinateur)
    {
        Ordinateur = ordinateur;
    }
}