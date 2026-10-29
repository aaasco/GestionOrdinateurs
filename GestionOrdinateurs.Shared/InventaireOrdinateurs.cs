namespace GestionOrdinateurs.Shared;

// Classe représentant un inventaire d'ordinateurs
public class InventaireOrdinateurs
{
    // Liste des ordinateurs de l'inventaire
    public List<Ordinateur> Ordinateurs { get; set; } = new();

    // Recherche un ordinateur par son nom
    public Ordinateur? this[string nom]
    {
        get
        {
            return Ordinateurs.FirstOrDefault(o => o.Nom == nom);
        }
    }

    // Récupère un ordinateur par sa position dans la liste
    public Ordinateur? this[int index]
    {
        get
        {
            if (index >= 0 && index < Ordinateurs.Count)
            {
                return Ordinateurs[index];
            }

            return null;
        }
    }
}