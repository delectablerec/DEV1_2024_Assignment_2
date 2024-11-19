using Microsoft.AspNetCore.Mvc;

public class ProdottiController : Controller
{
    private readonly ILogger<ProdottiController> _logger;
    private readonly ApplicationDbContext _context;

    public ProdottiController(ApplicationDbContext context, ILogger<ProdottiController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public IActionResult Index(int? minPrezzo, int? maxPrezzo, int? categoriaSelezionata, int? marcaSelezionata, int? materialeSelezionato, int? tipologiaSelezionata, int paginaCorrente = 1)
    {
        // Numero di default dei prodotti per pagina
        int prodottiPerPagina = 6;

        // Recupera tutte le categorie per il dropdown
        var categorie = _context.Categorie.ToList();
        var marche = _context.Marche.ToList();
        var materiali = _context.Materiali.ToList();
        var tipologie = _context.Tipologie.ToList();

        var prodottiTotali = CaricaProdotti();

        var prodottiFiltrati = FiltraProdotti(prodottiTotali.ToList(), minPrezzo, maxPrezzo, categoriaSelezionata, marcaSelezionata, materialeSelezionato, tipologiaSelezionata);

        if (!prodottiTotali.Any())
        {
            // Se non ci sono prodotti, passiamo un ViewModel vuoto
            var viewModelNoProducts = new ProdottiViewModel
            {
                Orologi = new List<Orologio>(), // Lista vuota
                MinPrezzo = 0,
                MaxPrezzo = 0,
                NumeroPagine = 1,
                PaginaCorrente = 1,
                Categorie = categorie,
                Marche = marche,
                Materiali = materiali,
                Tipologie = tipologie
            };
            return View(viewModelNoProducts); // Mostra la vista con la lista vuota e il messaggio
        }
        // Prende il conteggio totale per la paginazione
        int quantitaProdotti = prodottiFiltrati.Count();

        // Applica la paginazione (Skip and Take)
        var prodotti = prodottiFiltrati
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
            MaxPrezzo = maxPrezzo ?? (prodottiPaginati.Any() ? prodottiPaginati.Max(p => p.Prezzo) : 0),
            NumeroPagine = numeroPagine,
            PaginaCorrente = paginaCorrente,
            Categorie = categorie, 
            Marche = marche,  
            Materiali = materiali,
            Tipologie = tipologie,
            CategoriaSelezionata = categoriaSelezionata,
            MarcaSelezionata = marcaSelezionata,
            MaterialeSelezionato = materialeSelezionato,
            TipologiaSelezionata = tipologiaSelezionata,
            ConteggioProdotti = quantitaProdotti
        };
        return View(viewModel);
    }

    public List<Orologio> FiltraProdotti(List<Orologio> prodottiTotali, int? minPrezzo, int? maxPrezzo, int? categoriaId, int? marcaId, int? materialeId, int? tipologiaId)
    {
        List<Orologio> prodottiFiltrati = new List<Orologio>(); 
        bool scartato;

        foreach (var prodotto in prodottiTotali)
        {
            scartato = false;
            if(minPrezzo.HasValue && prodotto.Prezzo <= minPrezzo.Value)
            {
                scartato = true;
            }
            if(maxPrezzo.HasValue && prodotto.Prezzo >= maxPrezzo.Value)
            {
                scartato = true;
            }
            if(categoriaId.HasValue && prodotto.CategoriaId != categoriaId)
            {
                scartato = true;
            }
            if(marcaId.HasValue && prodotto.MarcaId != marcaId)
            {
                scartato = true;
            }
            if(materialeId.HasValue && prodotto.MaterialeId != materialeId)
            {
                scartato = true;
            }
            if(tipologiaId.HasValue && prodotto.TipologiaId != tipologiaId)
            {
                scartato = true;
            }
            if(scartato == false)
            {
                prodottiFiltrati.Add(prodotto);
            }
        }
        return prodottiFiltrati;
    }

    public IActionResult AggiungiProdotto()
    {
        var viewModel = new AggiungiProdottoViewModel
        {
            Orologio = new Orologio(),
            Categorie = CaricaCategorie(),
            Marche = CaricaMarche(),
            Materiali = CaricaMateriali(),
            Tipologie = CaricaTipologie(),
            Generi = CaricaGeneri()
        };
        return View(viewModel);
    }

    [HttpPost]
    public IActionResult AggiungiProdotto(AggiungiProdottoViewModel viewModel)
    {
        // Log for debugging purposes
        _logger.LogInformation("Valore della categoria: " + viewModel.Orologio.Categoria);
        _logger.LogInformation("Valore del materiale: " + viewModel.Orologio.Materiale);
        _logger.LogInformation("Valore della marca: " + viewModel.Orologio.Marca);
        _logger.LogInformation("Valore della tipologia: " + viewModel.Orologio.Tipologia);
        _logger.LogInformation("Valore del genere: " + viewModel.Orologio.Genere);
/*
        // Check if the model is valid, including custom validation for password
        if (!ModelState.IsValid)
        {
            // Log validation errors for debugging purposes
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogError(error.ErrorMessage);  // Log each error
                }
                // Return the view with validation errors
                viewModel.Categorie = CaricaCategorie();
                viewModel.Marche = CaricaMarche();
                viewModel.Materiali = CaricaMateriali();
                viewModel.Tipologie = CaricaTipologie();
                viewModel.Generi = CaricaGeneri();
                return View(viewModel);
            }
        }
*/
        // Log information about the current product
        _logger.LogInformation("Product is valid, saving to database");

        try
        {
            _context.Orologi.Add(viewModel.Orologio);  // Entity Framework will handle the ID assignment
            _context.SaveChanges();  // Save changes asynchronously
            _logger.LogInformation("Product successfully saved.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error while saving the product: {ex.Message}", ex);
            // Optionally, you can return an error page or display an error message
            return View("Error");
        }

        // Redirect to Index page after success
        _logger.LogInformation("Product successfully saved. Redirecting to Index.");
        return RedirectToAction("Index", "Home");
    }

    private List<Orologio> CaricaProdotti()
    {
        try
        {
            return _context.Orologi.ToList();  // Return all products as a List
        }
        catch (Exception ex)
        {
            _logger.LogError("Errore nella lettura : {Message} \n Exception Type : {ExceptionType} \n Stack Trace : {StackTrace}", ex.Message , ex.GetType().Name , ex.StackTrace);
            return new List<Orologio>(); // Return an empty list in case of error
        }
    }

    private List<Categoria> CaricaCategorie()
    {
        List<Categoria> categorieTotali = new List<Categoria>();
        try
        {
            foreach (Categoria categoria in _context.Categorie)
            {
                categorieTotali.Add(categoria); 
            }
            return categorieTotali;
        }
        catch (Exception ex)
        {
            _logger.LogError("Errore nella lettura : {Message} \n Exception Type : {ExceptionType} \n Stack Trace : {StackTrace}", ex.Message , ex.GetType().Name , ex.StackTrace);
            return new List<Categoria>(); // Ritorna una lista vuota se c'è un errore
        }
    } 

    private List<Marca> CaricaMarche()
    {
        List<Marca> marcheTotali = new List<Marca>();
        try
        {
            foreach (Marca marca in _context.Marche)
            {
                marcheTotali.Add(marca); 
            }
            return marcheTotali;
        }
        catch (Exception ex)
        {
            _logger.LogError("Errore nella lettura : {Message} \n Exception Type : {ExceptionType} \n Stack Trace : {StackTrace}", ex.Message , ex.GetType().Name , ex.StackTrace);
            return new List<Marca>();
        }
    } 

    private List<Materiale> CaricaMateriali()
    {
        List<Materiale> materialiTotali = new List<Materiale>();
        try
        {
            foreach (Materiale materiale in _context.Materiali)
            {
                materialiTotali.Add(materiale); 
            }
            return materialiTotali;
        }
        catch (Exception ex)
        {
            _logger.LogError("Errore nella lettura : {Message} \n Exception Type : {ExceptionType} \n Stack Trace : {StackTrace}", ex.Message , ex.GetType().Name , ex.StackTrace);
            return new List<Materiale>();
        }
    } 

    private List<Tipologia> CaricaTipologie()
    {
        List<Tipologia> tipologieTotali = new List<Tipologia>();
        try
        {
            foreach (Tipologia tipologia in _context.Tipologie)
            {
                tipologieTotali.Add(tipologia); 
            }
            return tipologieTotali;
        }
        catch (Exception ex)
        {
            _logger.LogError("Errore nella lettura : {Message} \n Exception Type : {ExceptionType} \n Stack Trace : {StackTrace}", ex.Message , ex.GetType().Name , ex.StackTrace);
            return new List<Tipologia>();
        }
    }

    private List<Genere> CaricaGeneri()
    {
        List<Genere> generiTotali = new List<Genere>();
        try
        {
            foreach (Genere genere in _context.Generi)
            {
                generiTotali.Add(genere); 
            }
            return generiTotali;
        }
        catch (Exception ex)
        {
            _logger.LogError("Errore nella lettura : {Message} \n Exception Type : {ExceptionType} \n Stack Trace : {StackTrace}", ex.Message , ex.GetType().Name , ex.StackTrace);
            return new List<Genere>();
        }
    }
    public IActionResult ModificaProdotto(int id)
    {
        var prodotti = CaricaProdotti();
        var prodotto = CercaProdottoPerId(prodotti, id);
        if (prodotto == null) return NotFound();
        var viewModel = new ModificaProdottoViewModel
        {
            Orologio = prodotto,
            Categorie = CaricaCategorie(),
            Marche = CaricaMarche(),
            Materiali = CaricaMateriali(),
            Tipologie = CaricaTipologie(),
            Generi = CaricaGeneri()
        };
        return View(viewModel);
    }

    [HttpPost]
    public IActionResult ModificaProdotto(ModificaProdottoViewModel viewModel)
    {
        _logger.LogInformation("Categoria selezionata: " + viewModel.Orologio.Categoria);
        var prodotti = _context.Orologi.ToList();
        var prodottoDaModificare = CercaProdottoPerId(prodotti, viewModel.Orologio.Id);
        if (prodottoDaModificare == null)
        {
            _logger.LogWarning("Prodotto con ID: {Id} non trovato.", viewModel.Orologio.Id);
            return NotFound();
        }
        
        _logger.LogInformation("Modifica del prodotto con ID: {Id}", viewModel.Orologio.Id);
        prodottoDaModificare.Nome = viewModel.Orologio.Nome;
        prodottoDaModificare.Prezzo = viewModel.Orologio.Prezzo;
        prodottoDaModificare.Giacenza = viewModel.Orologio.Giacenza;
        prodottoDaModificare.Colore = viewModel.Orologio.Colore;
        prodottoDaModificare.UrlImmagine = viewModel.Orologio.UrlImmagine;
        prodottoDaModificare.CategoriaId = viewModel.Orologio.CategoriaId;
        prodottoDaModificare.MarcaId = viewModel.Orologio.MarcaId;
        prodottoDaModificare.Modello = viewModel.Orologio.Modello;
        prodottoDaModificare.Referenza = viewModel.Orologio.Referenza;
        prodottoDaModificare.MaterialeId = viewModel.Orologio.MaterialeId;
        prodottoDaModificare.TipologiaId = viewModel.Orologio.TipologiaId;
        prodottoDaModificare.Diametro = viewModel.Orologio.Diametro;
        prodottoDaModificare.GenereId = viewModel.Orologio.GenereId;
        _logger.LogInformation("Prodotto con ID: {Id} modificato con successo.", viewModel.Orologio.Id);        
        try
        {
            _context.SaveChanges();
            _logger.LogInformation("Prodotto con ID: {Id} modificato con successo.", viewModel.Orologio.Id);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError("Errore nel salvataggio: {Message} \n Exception Type: {ExceptionType} \n Stack Trace: {StackTrace}", ex.Message, ex.GetType().Name, ex.StackTrace);
            return StatusCode(500, "Errore durante il salvataggio del prodotto.");
        }
    }

    private Orologio CercaProdottoPerId(List<Orologio> orologi, int id)
    {
        try
        {
            Orologio orologio = null;
    
            // Loop through the Orologi collection to find the product
            foreach (var item in orologi)
            {
                if (item.Id == id)
                {
                    orologio = item;
                    break; // Exit the loop once the product is found
                }
            }
            return orologio;
        }
        catch (Exception ex)
        {
            _logger.LogError("Errore nella ricerca : {Message} \n Exception Type : {ExceptionType} \n Stack Trace : {StackTrace}", ex.Message , ex.GetType().Name , ex.StackTrace);
            return null;
        }
    }

    public IActionResult EliminaProdotto(int id)
    {
        var prodotti = CaricaProdotti();
        var prodotto = CercaProdottoPerId(prodotti, id);
        if (prodotto == null)
        {
            // Se il prodotto non viene trovato
            return NotFound();
        }

        // Prepara il viewnodel con il prodotto da cancellare
        var viewModel = new EliminaProdottoViewModel
        {
            Orologio = prodotto
        };
        return View(viewModel);
    }

    [HttpPost, ActionName("EliminaProdotto")]
    public IActionResult EliminaProdottoEseguito(int id)
    {
        try
        {
            var prodotti = CaricaProdotti();
            var prodotto = CercaProdottoPerId(prodotti, id);
            if (prodotto == null)
            {
                return NotFound();
            }

            // Rimuove il prodotto da DbContext
            _context.Orologi.Remove(prodotto);
            
            // Salva le modifiche nel database
            _context.SaveChanges();
            _logger.LogInformation("Prodotto con ID: {Id} eliminato con successo.", id);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError("Errore nel salvataggio: {Message} \n Exception Type: {ExceptionType} \n Stack Trace: {StackTrace}", ex.Message, ex.GetType().Name, ex.StackTrace);
            return StatusCode(500, "Errore durante l'eliminazione del prodotto.");
        }
    }
}