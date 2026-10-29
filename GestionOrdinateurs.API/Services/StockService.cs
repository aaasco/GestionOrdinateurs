namespace GestionOrdinateurs.API.Services;

// Service de gestion du stock d'ordinateurs
public class StockService
{
    // Stock initial de 10 SSD
    public int StockSSD = 10;
    private readonly object verrou = new object(); // Objet pour synchroniser l'accès au stock

    // Méthode pour consommer un SSD du stock
    public async void ConsommerSSD()
    {
        Task.Delay(10).Wait();
        
        lock (verrou)
        {
            StockSSD--;
        }
    }
}