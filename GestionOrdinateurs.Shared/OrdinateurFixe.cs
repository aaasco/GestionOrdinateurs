namespace GestionOrdinateurs.Shared;

// Représente un ordinateur fixe avec des propriétés spécifiques
public class OrdinateurFixe : Ordinateur
{
    public bool PossedeWifi { get; set; }
    public int NombreEcrans { get; set; }

    // Indique si l'ordinateur fixe a plusieurs écrans
    public bool EstMultiEcran()
    {
        return NombreEcrans > 1;
    }

    // Indique si l'ordinateur fixe possède une connexion Wi-Fi
    public bool EstConnecteWifi()
    {
        return PossedeWifi;
    }
}