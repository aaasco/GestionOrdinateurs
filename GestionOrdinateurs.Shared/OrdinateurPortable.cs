namespace GestionOrdinateurs.Shared;

// Représente un ordinateur portable avec des propriétés spécifiques
public class OrdinateurPortable : Ordinateur
{
    public int AutonomieBatterie { get; set;}
    public double TailleEcran { get; set;}

    // Indique si l'autonomie restante de la batterie est faible (moins de 2 heures)
    public bool BatterieFaible()
    {
        return AutonomieBatterie < 2;
    }

    // Indique si l'ordinateur portable fait moins de 17 pouces
    public bool EstGrandEcran()
    {
        return TailleEcran >= 17;
    }
}