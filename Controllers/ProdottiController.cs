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

    public async Task<IActionResult> Index(int? minPrezzo, int? maxPrezzo, int? categoriaId, int? marcaId, int? materialeId, int? tipologiaId, int paginaCorrente = 1)
    {
        // Numero di default dei prodotti per pagina
        int prodottiPerPagina = 6;

        // Recupera tutte le categorie per il dropdown
        var categorie = await _context.Categorie.ToListAsync();
        var marche = await _context.Marche.ToListAsync();
        var materiali = await _context.Materiali.ToListAsync();
        var tipologie = await _context.Tipologie.ToListAsync();

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

        // Filtra per categoria se selezionata
        if (categoriaId.HasValue)
        {
            query = from o in query
                    where o.Categoria.Id == categoriaId.Value // !!! CONTROLLARE MODELLO PRODOTTO E VIEWMODEL PRODOTTI (CategoriaId da aggiungere sopra Categoria Categoria)
                    select o;
        }

        if (marcaId.HasValue)
        {
            query = from o in query
                    where o.Marca.Id == marcaId.Value // !!! CONTROLLARE MODELLO PRODOTTO E VIEWMODEL PRODOTTI (MarcaId da aggiungere sopra Marca Marca)
                    select o;
        }

        if (materialeId.HasValue)
        {
            query = from o in query
                    where o.Materiale.Id == materialeId.Value // !!! CONTROLLARE MODELLO OROLOGIO E VIEWMODEL PRODOTTI (MaterialeId da aggiungere sopra Materiale Materiale)
                    select o;
        }

        if (tipologiaId.HasValue)
        {
            query = from o in query
                    where o.Tipologia.Id == tipologiaId.Value  // !!! CONTROLLARE MODELLO OROLOGIO E VIEWMODEL PRODOTTI (TipologiaId da aggiungere sopra Tipologia Tipologia)
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
            PaginaCorrente = paginaCorrente,
            Categorie = categorie,  // Passa le categorie per il dropdown
            CategoriaSelezionata = categoriaId, // Passa la categoria selezionata, se presente   !!! DA AGGIUNGERE A VIEWMODEL
            Marche = marche,    // Passa le marche per il dropdown
            MarcaSelezionata = marcaId,  // Passa la marca selezionata, se presente  !!! DA AGGIUNGERE A VIEWMODEL
            Materiali = materiali,
            MaterialeSelezionato = materialeId, // DA AGGIUNGERE A VIEWMODEL
            Tipologie = tipologie,
            TipologiaSelezionata = tipologiaId // DA AGGIUNGERE A VIEWMODEL
        };

        return View(viewModel);
    }
}