namespace GestionOrdinateurs.API.Services;

// Service permettant d'écrire des messages dans un fichier journal
public class JournalService
{
    // Écrit un message dans le fichier de journalisation
    public void Ecrire(string message)
    {
        using var writer = new StreamWriter("reparations.log", append: true);
        writer.WriteLine(message);
    }
}