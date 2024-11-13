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
        var categorie = _context.Categorie.ToList();
        var marche = _context.Marche.ToList();
        var materiali = _context.Materiali.ToList();
        var tipologie = _context.Tipologie.ToList();
        
        // Inizia a costruire la query
        List<Orologio> prodottiTotali = new List<Orologio>();
        foreach (Orologio orologio in _context.Orologi)
        {
            prodottiTotali.Add(orologio); 
        }
        // Prende il conteggio totale per la paginazione
        int quantitaProdotti = prodottiTotali.Count();




        // Applica la paginazione (Skip and Take)
        var prodotti = FiltraProdotti(prodottiTotali, minPrezzo, maxPrezzo, categoriaId, marcaId, materialeId, tipologiaId)
                            .Skip((paginaCorrente - 1) * prodottiPerPagina) // Salta gli oggetti della pagina corrente
                            .Take(prodottiPerPagina) // Prende gli oggetti della pagina corrente
                            .ToList(); // Esegue la query in modo asincrono
        
        var prodottiPaginati = prodotti.OrderBy(o => o.Prezzo).ToList();

        // Calcola il numero totale di pagine
        int numeroPagine = (int)Math.Ceiling((double)quantitaProdotti / prodottiPerPagina);

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

    public List<Orologio> FiltraProdotti(List<Orologio> prodottiTotali, int? minPrezzo, int? maxPrezzo, int? categoriaId, int? marcaId, int? materialeId, int? tipologiaId)
    {
        List<Orologio> prodottiFiltrati = new List<Orologio>(); 
        bool aggiunto;
        
    }
}