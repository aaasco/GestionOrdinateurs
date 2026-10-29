namespace GestionOrdinateurs.Shared;
using System.Text.Json.Serialization;

// Classe de base représentant un ordinateur
public class Ordinateur
{
    // L'identifiant unique de l'ordinateur, défini lors de l'initialisation et ne peut pas être modifié par la suite
    public int Id { get; init;}
    public string Nom { get; set;} = "";
    public EtatOrdinateur Etat { get; set; } = EtatOrdinateur.EnService;
    public int Stock { get; set; } = 0;
    public decimal Prix { get; set; } = 0;
    public bool Vendu { get; set; } = false;

    // Caractéristiques de l'ordinateur
    public Caracteristiques Caracteristiques { get; set; } = null!;

    // Historique des réparations de l'ordinateur
    [JsonIgnore] // Ignore cette propriété lors de la sérialisation JSON pour éviter les références circulaires
    public List<Reparation> Reparations { get; set; } = new();

    //Passe l'ordinateur en maintenance
    public void MettreEnMaintenance()
    {
        Etat = EtatOrdinateur.EnMaintenance;
    }

    //Vérifie si l'ordinateur est disponible
    public bool EstDisponible()
    {
        return Etat == EtatOrdinateur.EnService;
    }

    //Modifie l'état de l'ordinateur en fonction de l'état passé en paramètre
    public void ModifierEtat(EtatOrdinateur nouvelEtat)
    {
        Etat = nouvelEtat;
    }
}