public class ProdottiService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProdottiService> _logger;

    public ProdottiService(ApplicationDbContext context, ILogger<ProdottiService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public List<Orologio> CaricaProdotti()
    {
        try
        {
            return _context.Orologi.ToList();  // Return all products as a List
        }
        catch (Exception ex)
        {
            _logger.LogError("Errore nella lettura : {Message} \n Exception Type : {ExceptionType} \n Stack Trace : {StackTrace}", ex.Message , ex.GetType().Name , ex.StackTrace);
            return new List<Orologio>();
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

    public List<Orologio> FiltraProdotti(
        List<Orologio> prodottiTotali, int? minPrezzo, int? maxPrezzo, int? categoriaId, int? marcaId,
        int? materialeId, int? tipologiaId)
    {
        List<Orologio> prodottiFiltrati = new List<Orologio>(); 
        bool scartato;

        foreach (var prodotto in prodottiTotali)
        {
            scartato = false;
            if(minPrezzo.HasValue && prodotto.Prezzo < minPrezzo.Value)
            {
                scartato = true;
            }
            if(maxPrezzo.HasValue && prodotto.Prezzo > maxPrezzo.Value)
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

    public ProdottiViewModel PreparaProdottiViewModel(
        int? minPrezzo, int? maxPrezzo, int? categoriaId, int? marcaId, int? materialeId, int? tipologiaId, 
        int paginaCorrente, int prodottiPerPagina)
    {
        var categorie = _context.Categorie.ToList();
        var marche = _context.Marche.ToList();
        var materiali = _context.Materiali.ToList();
        var tipologie = _context.Tipologie.ToList();

        var prodottiTotali = CaricaProdotti();
        var prodottiFiltrati = FiltraProdotti(prodottiTotali, minPrezzo, maxPrezzo, categoriaId, marcaId, materialeId, tipologiaId);

        int quantitaProdotti = prodottiFiltrati.Count;
        int numeroPagine = (int)Math.Ceiling((double)quantitaProdotti / prodottiPerPagina);

        var prodottiPaginati = prodottiFiltrati
            .Skip((paginaCorrente - 1) * prodottiPerPagina)
            .Take(prodottiPerPagina)
            .OrderBy(p => p.Prezzo)
            .ToList();

        return new ProdottiViewModel
        {
            Orologi = prodottiPaginati,
            MinPrezzo = minPrezzo ?? 0,
            MaxPrezzo = maxPrezzo ?? (prodottiFiltrati.Any() ? prodottiFiltrati.Max(p => p.Prezzo) : 0),
            NumeroPagine = numeroPagine,
            PaginaCorrente = paginaCorrente,
            Categorie = categorie,
            Marche = marche,
            Materiali = materiali,
            Tipologie = tipologie,
            CategoriaSelezionata = categoriaId,
            MarcaSelezionata = marcaId,
            MaterialeSelezionato = materialeId,
            TipologiaSelezionata = tipologiaId,
            ConteggioProdotti = quantitaProdotti
        };
    }
    public AggiungiProdottoViewModel PreparaAggiungiProdottoViewModel()
    {
        return new AggiungiProdottoViewModel
        {
            Orologio = new Orologio(),
            Categorie = CaricaCategorie(),
            Marche = CaricaMarche(),
            Materiali = CaricaMateriali(),
            Tipologie = CaricaTipologie(),
            Generi = CaricaGeneri()
        };
    }

    public bool SalvaProdotto(Orologio orologio)
    {
        try
        {
            _context.Orologi.Add(orologio);
            _context.SaveChanges();
            _logger.LogInformation("Product successfully saved.");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error while saving the product: {ex.Message}", ex);
            return false;
        }
    }

    public ModificaProdottoViewModel PreparaModificaProdottoViewModel(int id)
    {
        var prodotti = CaricaProdotti();
        var prodotto = CercaProdottoPerId(prodotti, id);
        
        if (prodotto == null)
        {
            _logger.LogWarning("Prodotto con ID: {Id} non trovato.", id);
            return null;
        }

        return new ModificaProdottoViewModel
        {
            Orologio = prodotto,
            Categorie = CaricaCategorie(),
            Marche = CaricaMarche(),
            Materiali = CaricaMateriali(),
            Tipologie = CaricaTipologie(),
            Generi = CaricaGeneri()
        };
    }

    public bool ModificaProdotto(Orologio prodottoModificato)
    {
        var prodottoEsistente = CercaProdottoPerId(_context.Orologi.ToList(), prodottoModificato.Id);

        if (prodottoEsistente == null)
        {
            _logger.LogWarning("Prodotto con ID: {Id} non trovato.", prodottoModificato.Id);
            return false;
        }

        prodottoEsistente.Nome = prodottoModificato.Nome;
        prodottoEsistente.Prezzo = prodottoModificato.Prezzo;
        prodottoEsistente.Giacenza = prodottoModificato.Giacenza;
        prodottoEsistente.Colore = prodottoModificato.Colore;
        prodottoEsistente.UrlImmagine = prodottoModificato.UrlImmagine;
        prodottoEsistente.CategoriaId = prodottoModificato.CategoriaId;
        prodottoEsistente.MarcaId = prodottoModificato.MarcaId;
        prodottoEsistente.Modello = prodottoModificato.Modello;
        prodottoEsistente.Referenza = prodottoModificato.Referenza;
        prodottoEsistente.MaterialeId = prodottoModificato.MaterialeId;
        prodottoEsistente.TipologiaId = prodottoModificato.TipologiaId;
        prodottoEsistente.Diametro = prodottoModificato.Diametro;
        prodottoEsistente.GenereId = prodottoModificato.GenereId;

        try
        {
            _context.SaveChanges();
            _logger.LogInformation("Prodotto con ID: {Id} modificato con successo.", prodottoModificato.Id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Errore nel salvataggio del prodotto con ID: {prodottoModificato.Id} - {ex.Message}", ex);
            return false;
        }
    }

    public EliminaProdottoViewModel PreparaEliminaProdottoViewModel(int id)
    {
        try
        {
            // Retrieve the product from the database
            var prodotto = CercaProdottoPerId(_context.Orologi.ToList(), id);

            if (prodotto == null)
            {
                return null; // Return null if the product was not found
            }

            // Prepare the ViewModel with the product to delete
            var viewModel = new EliminaProdottoViewModel
            {
                Orologio = prodotto
            };

            return viewModel;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error preparing ViewModel for product deletion: {ex.Message}");
            return null; // Return null in case of an error
        }
    }

    public bool EliminaProdotto(int id)
    {
        try
        {
            // Find the product to delete
            Orologio prodotto = CercaProdottoPerId(_context.Orologi.ToList(), id);
            if (prodotto == null)
            {
                return false; // Product not found
            }

            // Remove the product from the DbContext
            _context.Orologi.Remove(prodotto);
            
            // Save changes in the database
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting product: {ex.Message}");
            return false;
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
}