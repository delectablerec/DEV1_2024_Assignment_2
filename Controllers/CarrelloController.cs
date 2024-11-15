using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


public class CarrelloController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProdottiController> _logger;
    private CarrelloViewModel Carrello;

    public CarrelloController(ApplicationDbContext context, ILogger<ProdottiController> logger)
    {
        _context = context;
        _logger = logger;
    }
    public IActionResult Index()
    {
        Carrello = new CarrelloViewModel
        {
            Carrello = new List<Orologio>(),  // Se necessario, inizializza qui
            Totale = 0,
            Quantita = 0
        };

        // Popola il carrello se hai dei dati, ad esempio:
        // viewModel.Carrello.Add(new Orologio { Nome = "Esempio", Prezzo = 100, Descrizione = "Descrizione esempio", ImmagineUrl = "/path/to/image" });

        return View(Carrello);
    }

        public IActionResult AggiungiACarrello(int id)
    {
        try
        {
            Carrello.Carrello.Add(_context.Orologi.ToList().Find(p => p.Id == id));
        }
        catch (Exception ex)
        {
            _logger.LogError("Errore nella ricerca : {Message} \n Exception Type : {ExceptionType} \n Stack Trace : {StackTrace}", ex.Message , ex.GetType().Name , ex.StackTrace);
            return null;
        }
        return View();
    }
}
