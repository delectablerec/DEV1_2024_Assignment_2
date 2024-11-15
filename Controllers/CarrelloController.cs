using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


public class CarrelloController : Controller
{
    private readonly ApplicationDbContext _context;

    public CarrelloController(ApplicationDbContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        var viewModel = new CarrelloViewModel
        {
            Carrello = new List<Orologio>(),  // Se necessario, inizializza qui
            Totale = 0,
            Quantita = 0
        };

        // Popola il carrello se hai dei dati, ad esempio:
        // viewModel.Carrello.Add(new Orologio { Nome = "Esempio", Prezzo = 100, Descrizione = "Descrizione esempio", ImmagineUrl = "/path/to/image" });

        return View(viewModel);
    }
}
