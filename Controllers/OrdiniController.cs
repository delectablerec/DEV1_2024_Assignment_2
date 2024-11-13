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
    public IActionResult Index()
{
    var ordini = _context.Ordini.ToList();

    // Crea il ViewModel e assegna la lista di ordini
    var viewModel = new OrdiniViewModel
    {
        Ordini = ordini
    };

    return View(viewModel); // Passa il ViewModel alla vista
}*/

public IActionResult Index()
{
    // Crea una lista vuota per contenere gli ordini con i prodotti associati
    List<Ordine> ordiniConProdotti = new List<Ordine>();

    // Itera su tutti gli ordini nel contesto (database)
    foreach (var ordine in _context.Ordini)
    {
        // Crea una lista per gli orologi associati all'ordine corrente
        List<Orologio> prodottiAssociati = new List<Orologio>();

        // Trova gli orologi associati all'ordine corrente
        foreach (var orologio in _context.Orologi)
        {
            // Controlla se l'orologio è incluso nell'ordine
            bool orologioAssociato = false;

            foreach (var ordineAssociato in orologio.Ordini)
            {
                if (ordineAssociato.Id == ordine.Id)
                {
                    orologioAssociato = true;
                    break;
                }
            }

            // Se l'orologio è associato all'ordine, aggiungilo alla lista dei prodotti associati
            if (orologioAssociato)
            {
                prodottiAssociati.Add(orologio);
            }
        }

        // Assegna la lista di prodotti associati all'ordine
        ordine.Orologi = prodottiAssociati;

        // Aggiungi l'ordine con i prodotti associati alla lista
        ordiniConProdotti.Add(ordine);
    }

    // Crea il ViewModel e assegna la lista di ordini
    var viewModel = new OrdiniViewModel
    {
        Ordini = ordiniConProdotti
    };

    // Passa il ViewModel alla vista "Index"
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
    }

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