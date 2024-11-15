using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

public class OrdiniController : Controller
{
    private readonly ApplicationDbContext _context;

    public OrdiniController(ApplicationDbContext context)
    {
        _context = context;
    }

/*

// Visualizza l'elenco di ordini
public IActionResult Index()
{
    // Recupera tutti gli ordini e i relativi prodotti
    //Include("Orologi") Include gli orologi associati
    // Include il cliente associato
    var ordini = _context.Ordini.Include("Orologi").Include("Cliente").ToList();

    // Crea il ViewModel e assegna direttamente la lista di ordini Ordini
    var viewModel = new ListaOrdiniViewModel
    {
        ListaOrdiniViewModel = ordini
    };

    // Passa il ViewModel alla vista
    return View(viewModel);
}

public IActionResult ElencoOrdini()
{
    // Recupera tutti gli ordini dal database
    var ordini = _context.Ordini.Include(o => o.Orologi).ToList();

    // Popola il ViewModel
    var viewModel = ordini.Select(ordine => new ListaOrdiniViewModel
    {
        Id = ordine.Id,
        NomeOrdine = ordine.Nome,
        DataAcquisto = ordine.DataAcquisto,
        StatoOrdine = ordine.StatoOrdine,
        TotaleOrdine = ordine.Orologi.Sum(o => o.Prezzo), // Calcolo del totale dei prodotti
        UrlImmagineProdotto = ordine.Orologi.FirstOrDefault()?.UrlImmagine,
        NomeProdotto = ordine.Orologi.FirstOrDefault()?.Nome,
        CostoSpedizione = 10.00m // Sostituisci con il valore reale
    }).ToList();

    return View(viewModel);
}




    // Azione GET per visualizzare il form di aggiunta di un nuovo ordine
    [HttpGet]
    public IActionResult AddOrder()
    {
        return View();
    }

    // Azione POST per aggiungere un nuovo ordine
    [HttpPost]
    public async Task<IActionResult> AddOrder(Ordine ordine)
    {
        if (ModelState.IsValid)
        {
            _context.Ordini.Add(ordine);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        return View(ordine); // Ritorna alla vista di aggiunta ordine in caso di errore di validazione
    }*/

    // Azione per eliminare ordine

[HttpPost]
[Authorize(Roles = "Admin")]
public IActionResult DeleteOrder(int id)
{
    Ordine ordine = null;

    foreach (var ord in _context.Ordini)
    {
        if (ord.Id == id)
        {
            ordine = ord;
            break;
        }
    }

    if (ordine != null)
    {
        _context.Ordini.Remove(ordine);
        _context.SaveChanges(); // Chiamata sincrona a SaveChanges()
        return RedirectToAction("Index");
    }

    return NotFound(); // Restituisce 404 se l'ordine non è trovato
}



}