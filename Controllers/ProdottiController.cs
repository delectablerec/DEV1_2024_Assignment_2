using Microsoft.AspNetCore.Mvc;

public class ProdottiController : Controller
{
    private ProdottiService _prodottiService;
    private readonly ILogger<ProdottiController> _logger;
    private readonly ApplicationDbContext _context;


    public ProdottiController(ApplicationDbContext context, ILogger<ProdottiController> logger, ProdottiService prodottiService)
    {
        _context = context;
        _logger = logger;
        _prodottiService = prodottiService;
    }

    public IActionResult Index(int? minPrezzo, int? maxPrezzo, int? categoriaSelezionata, int? marcaSelezionata, int? materialeSelezionato, int? tipologiaSelezionata, int paginaCorrente = 1)
    {
        const int prodottiPerPagina = 6;

        var viewModel = _prodottiService.PreparaProdottiViewModel(
            minPrezzo, maxPrezzo, categoriaSelezionata, marcaSelezionata, materialeSelezionato, tipologiaSelezionata,
            paginaCorrente, prodottiPerPagina);

        return View(viewModel);
    }

    public IActionResult AggiungiProdotto()
    {
        var viewModel = _prodottiService.PreparaAggiungiProdottoViewModel();
        return View(viewModel);
    }

    [HttpPost]
    public IActionResult AggiungiProdotto(AggiungiProdottoViewModel viewModel)
    {
        /*if (!ModelState.IsValid)
        {
            _logger.LogWarning("Validation failed for the product.");
            viewModel = _prodottiService.PreparaAggiungiProdottoViewModel();
            return View(viewModel);
        }*/

        _logger.LogInformation("Attempting to save product.");
        var isSaved = _prodottiService.SalvaProdotto(viewModel.Orologio);

        if (!isSaved)
        {
            return View("Error"); // Optionally show an error view
        }

        _logger.LogInformation("Product saved. Redirecting to Index.");
        return RedirectToAction("Index", "Home");
    }

    public IActionResult ModificaProdotto(int id)
    {
        var viewModel = _prodottiService.PreparaModificaProdottoViewModel(id);

        if (viewModel == null)
        {
            return NotFound();
        }

        return View(viewModel);
    }

    [HttpPost]
    public IActionResult ModificaProdotto(ModificaProdottoViewModel viewModel)
    {
        _logger.LogInformation("Categoria selezionata: " + viewModel.Orologio.Categoria);

        var modificato = _prodottiService.ModificaProdotto(viewModel.Orologio);

        if (!modificato)
        {
            return StatusCode(500, "Errore durante il salvataggio del prodotto.");
        }

        _logger.LogInformation("Prodotto con ID: {Id} modificato. Redirecting to Index.", viewModel.Orologio.Id);
        return RedirectToAction("Index");
    }
    
/*
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
*/
    public IActionResult EliminaProdotto(int id)
    {
        try
        {
            // Call the service to get the ViewModel for the product to delete
            var viewModel = _prodottiService.PreparaEliminaProdottoViewModel(id);
            
            if (viewModel == null)
            {
                // If the product is not found, return NotFound
                return NotFound();
            }
            
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving product: {ex.Message}");
            return StatusCode(500, "An error occurred while retrieving the product.");
        }
    }

    [HttpPost, ActionName("EliminaProdotto")]
    public IActionResult EliminaProdottoEseguito(int id)
    {
        try
        {
            // Call the service to delete the product
            bool success = _prodottiService.EliminaProdotto(id);

            if (success)
            {
                _logger.LogInformation("Product with ID {Id} successfully deleted.", id);
                return RedirectToAction("Index");
            }
            else
            {
                return NotFound();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting product: {ex.Message}");
            return StatusCode(500, "An error occurred while deleting the product.");
        }
    }
}