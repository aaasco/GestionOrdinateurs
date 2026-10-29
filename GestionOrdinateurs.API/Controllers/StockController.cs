
using Microsoft.AspNetCore.Mvc;
using GestionOrdinateurs.API.Services;

namespace GestionOrdinateurs.API.Controllers;

// Controller pour gérer le stock d'ordinateurs
[ApiController]
[Route("api/[controller]")]
public class StockController : ControllerBase
{
    private readonly StockService stockService;

    // Initialise le service utilisé par le controller
    public StockController(StockService stockService)
    {
        this.stockService = stockService;
    }

    // Lance 10 consommations de SSD en parallèle
    [HttpGet("GetStockSSD")]
    public IActionResult GetStockSSD()
    {
        Parallel.For(0, 10, i =>
        {
            stockService.ConsommerSSD();
        });

        Console.WriteLine($"Stock SSD restant : {stockService.StockSSD}");

        return Ok(stockService.StockSSD);
    }
}
