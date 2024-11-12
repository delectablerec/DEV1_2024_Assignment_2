using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
public class ProdottiController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProdottiController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int? minPrezzo, int? maxPrezzo, int paginaCorrente = 1)
    {
        // Numero di default dei prodotti per pagina
        int prodottiPerPagina = 6;

        // Inizia a costruire la query
        var query = from o in _context.Orologi
                    select o;

        // Applica il prezzo minimo se fornito
        if (minPrezzo.HasValue)
        {
            query = from o in query
                    where o.Prezzo >= minPrezzo.Value
                    select o;
        }

        // Applica il prezzo massimo se fornito
        if (maxPrezzo.HasValue)
        {
            query = from o in query
                    where o.Prezzo <= maxPrezzo.Value
                    select o;
        }

        // Prende il conteggio totale per la paginazione
        int totalProdotti = await query.CountAsync();

        // Applica la paginazione (Skip and Take)
        var prodottiPaginati = await (from o in query
                                    orderby o.Prezzo // !! Qua si può opzionalmente aggiungere l'ordine
                                    select o)
                                    .Skip((paginaCorrente - 1) * prodottiPerPagina) // Salta gli oggetti della pagina corrente
                                    .Take(prodottiPerPagina) // Prende gli oggetti della pagina corrente
                                    .ToListAsync(); // Esegue la query in modo asincrono

        // Calcola il numero totale di pagine
        int numeroPagine = (int)Math.Ceiling((double)totalProdotti / prodottiPerPagina);

        // Prepara il viewmodel
        var viewModel = new ProdottiViewModel
        {
            Orologi = prodottiPaginati,
            MinPrezzo = minPrezzo ?? 0,
            MaxPrezzo = (from o in _context.Orologi
                        select o.Prezzo).Max(),
            NumeroPagine = numeroPagine,
            PaginaCorrente = paginaCorrente
        };

        return View(viewModel);
    }
}