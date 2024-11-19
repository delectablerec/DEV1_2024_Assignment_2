using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public class ProdottiController : Controller
{
    private ProdottiService _prodottiService;
    private readonly ILogger<ProdottiController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<Cliente> _userManager; // Cliente eredita da IdentityUser
    private CarrelloService _carrelloService;


    public ProdottiController(ApplicationDbContext context, ILogger<ProdottiController> logger, ProdottiService prodottiService, UserManager<Cliente> userManager, CarrelloService carrelloService)
    {
        _context = context;
        _logger = logger;
        _prodottiService = prodottiService;
        _userManager = userManager;
        _carrelloService = carrelloService;
    }

    public IActionResult Index(int? minPrezzo, int? maxPrezzo, int? categoriaSelezionata, int? marcaSelezionata, int? materialeSelezionato, int? tipologiaSelezionata, int paginaCorrente = 1)
    {
        const int prodottiPerPagina = 6;

        // Per assegnare le variabili passate al viewmodel
        var viewModel = _prodottiService.PreparaProdottiViewModel(
            minPrezzo, maxPrezzo, categoriaSelezionata, marcaSelezionata, materialeSelezionato, tipologiaSelezionata,
            paginaCorrente, prodottiPerPagina);

        ViewData["CartItemCount"] = ItemsInCart();
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

        _logger.LogInformation("Tentativo di salvare il prodotto.");
        // Bool per prodotto salvato o meno
        var salvato = _prodottiService.SalvaProdotto(viewModel.Orologio);

        if (!salvato)
        {
            return View("Error");
        }

        _logger.LogInformation("Prodotto salvato. Redirecting a Index.");
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
    
    public IActionResult EliminaProdotto(int id)
    {
        try
        {
            var viewModel = _prodottiService.PreparaEliminaProdottoViewModel(id);
            
            if (viewModel == null)
            {
                return NotFound();
            }
            
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Errore nella ricerca del prodotto: {ex.Message}");
            return StatusCode(500, "Qualcosa è andato storto duranrte la ricerca del prodotto.");
        }
    }

    [HttpPost, ActionName("EliminaProdotto")]
    public IActionResult EliminaProdottoEseguito(int id)
    {
        try
        {
            bool success = _prodottiService.EliminaProdotto(id);

            if (success)
            {
                _logger.LogInformation("Prodotto con ID {Id} cancellato con successo.", id);
                return RedirectToAction("Index");
            }
            else
            {
                return NotFound();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Errore nella cancellazione del prodotto: {ex.Message}");
            return StatusCode(500, "Qualcosa è andato storto durante la cancellazione del prodotto.");
        }
    }
            public int ItemsInCart()
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogError("User ID is null or empty.");
            return 0;
        }

        var carrello = _carrelloService.CaricaCarrello(userId);

        if (carrello == null || carrello.Carrello.Count == 0)
        {
            _logger.LogWarning("Carrello vuoto per UserId: {UserId}", userId);
            return 0;
        }
        else
        {
            return carrello.Carrello.Count;
        }
    }
}