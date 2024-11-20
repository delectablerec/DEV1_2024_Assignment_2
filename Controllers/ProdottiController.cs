using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


public class ProdottiController : Controller
{
    private ProdottiService _prodottiService;
    private readonly ILogger<ProdottiController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _hostingEnvironment;
    private CarrelloService _carrelloService;


    public ProdottiController(ApplicationDbContext context, ILogger<ProdottiController> logger, ProdottiService prodottiService, CarrelloService carrelloService, IWebHostEnvironment hostingEnvironment)
    {
        _context = context;
        _logger = logger;
        _prodottiService = prodottiService;
        _hostingEnvironment = hostingEnvironment;
        _carrelloService = carrelloService;
    }

    public IActionResult Index(int? minPrezzo, int? maxPrezzo, int? categoriaSelezionata, int? marcaSelezionata, int? materialeSelezionato, int? tipologiaSelezionata, int paginaCorrente = 1)
    {
        const int prodottiPerPagina = 6;

        // Per assegnare le variabili passate al viewmodel
        var viewModel = _prodottiService.PreparaProdottiViewModel(
            minPrezzo, maxPrezzo, categoriaSelezionata, marcaSelezionata, materialeSelezionato, tipologiaSelezionata,
            paginaCorrente, prodottiPerPagina);

        ViewData["CartItemCount"] = _carrelloService.ItemsInCart(User);
        return View(viewModel);
    }

    public IActionResult DettaglioProdotto(int id)
    {
        var viewModel = _prodottiService.PreparaDettaglioProdottoViewModel(id);

        if (viewModel == null)
        {
            return NotFound();
        }

        return View(viewModel);
    }

    public IActionResult AggiungiProdotto()
    {
        var viewModel = _prodottiService.PreparaAggiungiProdottoViewModel();
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> AggiungiProdotto(AggiungiProdottoViewModel viewModel)
    {
        if (viewModel.ImmagineCaricata != null && viewModel.ImmagineCaricata.Length > 0)
        {
            // Salva l'immagine e ottieni l'URL
            var fileName = Path.GetFileName(viewModel.ImmagineCaricata.FileName);
            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await viewModel.ImmagineCaricata.CopyToAsync(stream);
            }

            // Assegna l'URL dell'immagine al prodotto
            viewModel.Orologio.UrlImmagine = $"/images/{fileName}";
        }

        // Salva il prodotto nel database
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
    public async Task<IActionResult> ModificaProdotto(ModificaProdottoViewModel viewModel)
    {
        if (viewModel.ImmagineCaricata != null && viewModel.ImmagineCaricata.Length > 0)
        {
            // Salva l'immagine e ottieni l'URL
            var fileName = Path.GetFileName(viewModel.ImmagineCaricata.FileName);
            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await viewModel.ImmagineCaricata.CopyToAsync(stream);
            }

            // Assegna l'URL dell'immagine al prodotto
            viewModel.Orologio.UrlImmagine = $"/images/{fileName}";
        }

        // Modifica il prodotto nel database
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
}