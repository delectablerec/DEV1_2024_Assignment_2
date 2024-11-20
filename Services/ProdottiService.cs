using Microsoft.EntityFrameworkCore;
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
            return _context.Orologi.ToList();  // Ritorna tutti i prodotti come lista
        }
        catch (Exception ex)
        {
            _logger.LogError("Errore nella lettura : {Message} \n Exception Type : {ExceptionType} \n Stack Trace : {StackTrace}", ex.Message , ex.GetType().Name , ex.StackTrace);
            return new List<Orologio>();    // Ritorna una lista vuota se c'è un errore
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
            return new List<Categoria>(); 
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
        List<Orologio> prodottiFiltrati = new List<Orologio>(); // Lista di prodotti filtrati
        bool scartato;

        foreach (var prodotto in prodottiTotali)
        {
            scartato = false;
            if(minPrezzo.HasValue && prodotto.Prezzo < minPrezzo.Value) // Se prezzo minore del prezzo minimo
            {
                scartato = true;
            }
            if(maxPrezzo.HasValue && prodotto.Prezzo > maxPrezzo.Value) // Se prezzo maggiore di prezzo massimo
            {
                scartato = true;
            }
            if(categoriaId.HasValue && prodotto.CategoriaId != categoriaId) // Se la categoria non corrisponde a quella selezionata
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
                scartato = true;    // Scarta il prodotto
            }
            if(scartato == false)   // Se prodotto non scartato
            {
                prodottiFiltrati.Add(prodotto); // Lo aggiunge alla lista
            }
        }
        return prodottiFiltrati;
    }

    public List<Orologio> OrdinaPerPrezzo(List<Orologio> prodotti)
    {
        // Bubble Sort per ordinare i prodotti per prezzo
        for (int i = 0; i < prodotti.Count - 1; i++)
        {
            for (int j = 0; j < prodotti.Count - 1 - i; j++)
            {
                if (prodotti[j].Prezzo > prodotti[j + 1].Prezzo)
                {
                    // Inverte gli elementi se sono nell'ordine sbagliato
                    var temp = prodotti[j];
                    prodotti[j] = prodotti[j + 1];
                    prodotti[j + 1] = temp;
                }
            }
        }

        return prodotti;
    }

    public ProdottiViewModel PreparaProdottiViewModel(
        int? minPrezzo, int? maxPrezzo, int? categoriaId, int? marcaId, int? materialeId, int? tipologiaId, 
        int paginaCorrente, int prodottiPerPagina)
    {
        var categorie = CaricaCategorie();
        var marche = CaricaMarche();
        var materiali = CaricaMateriali();
        var tipologie = CaricaTipologie();

        var prodottiTotali = CaricaProdotti();
        var prodottiFiltrati = FiltraProdotti(prodottiTotali, minPrezzo, maxPrezzo, categoriaId, marcaId, materialeId, tipologiaId);

        int quantitaProdotti = prodottiFiltrati.Count;  // Conteggio dei prodotti filtrati
        int numeroPagine = (int)Math.Ceiling((double)quantitaProdotti / prodottiPerPagina); // Numero pagine in base a quantità prodotti

        var prodottiOrdinati = OrdinaPerPrezzo(prodottiFiltrati);   // Per non usare .OrderBy(p => p.Prezzo)

        var prodottiPaginati = prodottiOrdinati // Inpaginazione dei prodotti
            .Skip((paginaCorrente - 1) * prodottiPerPagina)
            .Take(prodottiPerPagina)
            .ToList();

        return new ProdottiViewModel
        {
            Orologi = prodottiPaginati,
            MinPrezzo = minPrezzo ?? 0,
            MaxPrezzo = maxPrezzo ?? (prodottiFiltrati.Any() ? prodottiFiltrati.Max(p => p.Prezzo) : 0),    // Per problema pagina vuota per nessuna corrispondenza filtro
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

    public DettaglioProdottoViewModel PreparaDettaglioProdottoViewModel(int id)
    {
        try
        {
            // Trova il prodotto nel database includendo gli oggetti che ha come proprietà
            var prodotto = CercaProdottoPerId(_context.Orologi   
                                                    .Include(o => o.Categoria)
                                                    .Include(o => o.Marca)
                                                    .Include(o => o.Materiale)
                                                    .Include(o => o.Tipologia)
                                                    .Include(o => o.Genere)
                                                    .ToList(),id);

            if (prodotto == null)
            {
                return null; // Return null se prodotto non trovato
            }

            var viewModel = new DettaglioProdottoViewModel
            {
                Orologio = prodotto
            };

            return viewModel;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Errore nella preparazione del view model: {ex.Message}");
            return null; 
        }
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
            _context.Orologi.Add(orologio); // Aggiunge prodotto al database
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
            var prodotto = CercaProdottoPerId(_context.Orologi
                                                .Include(o => o.Marca) // Eagerly load the Marca property
                                                .ToList(),id);

            if (prodotto == null)
            {
                return null; // Return null se prodotto non trovato
            }

            var viewModel = new EliminaProdottoViewModel
            {
                Orologio = prodotto
            };

            return viewModel;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Errore nella preparazione del view model: {ex.Message}");
            return null; 
        }
    }

    public bool EliminaProdotto(int id)
    {
        try
        {
            Orologio prodotto = CercaProdottoPerId(_context.Orologi.ToList(), id);
            if (prodotto == null)
            {
                return false; 
            }

            // Rimuove il prodotto dal database
            _context.Orologi.Remove(prodotto);
            
            // Salva le modifiche
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Errore nella cancellazione del prodotto: {ex.Message}");
            return false;
        }
    }

    private Orologio CercaProdottoPerId(List<Orologio> orologi, int id)
    {
        try
        {
            Orologio orologio = null;
    
            // Loop nella collezione per trovare il prodotto
            foreach (var item in orologi)
            {
                if (item.Id == id)
                {
                    orologio = item;
                    break; // Esce dal loop una volta trovato
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