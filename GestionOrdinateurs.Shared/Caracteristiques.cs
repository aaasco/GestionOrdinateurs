namespace GestionOrdinateurs.Shared;

// Record contenant les caractéristiques techniques d'un ordinateur
public record Caracteristiques
{
    public int RamGo { get; set; }
    public TypeOrdinateur TypeOrdi { get; set; }
    public int StockageGo { get; set; }

    public string Marque { get; set; } = "";
    public string Processeur { get; set; } = "";

    // Initialise les caractéristiques de l'ordinateur
    public Caracteristiques(
        int ramGo,
        int stockageGo,
        TypeOrdinateur typeOrdi)
    {
        RamGo = ramGo;
        StockageGo = stockageGo;
        TypeOrdi = typeOrdi;
    }
}