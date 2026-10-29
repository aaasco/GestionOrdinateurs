using GestionOrdinateurs.Shared;
namespace GestionOrdinateurs.API.Events;

//Informations transmises lors de la suppression d'un ordinateur
public class OrdinateurSupprimeEventArgs : EventArgs
{
    public Ordinateur Ordinateur { get; }

//
    public OrdinateurSupprimeEventArgs(Ordinateur ordinateur)
    {
        Ordinateur = ordinateur;
    }
}