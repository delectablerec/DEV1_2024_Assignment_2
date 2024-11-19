using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
public class OrdiniController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly OrdiniService _ordiniService;
    private readonly ILogger<OrdiniController> _logger;
    private readonly UserManager<Cliente> _userManager;

    public OrdiniController(OrdiniService ordiniService, ILogger<OrdiniController> logger, UserManager<Cliente> userManager)
    {
        ApplicationDbContext context;
        _ordiniService = ordiniService;
        _logger = logger;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        try
        {
            var ordini = _ordiniService.GetOrdini();
            return View(ordini);
        }
        catch
        {
            return StatusCode(500, "Errore interno del server.");
        }
    }

    [HttpPost]
public IActionResult CreaOrdineDaCarrello()
{
    try
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("Utente non autenticato. Impossibile creare un ordine.");
            return Unauthorized("Devi essere autenticato per effettuare un ordine.");
        }

        var carrello = _ordiniService.CaricaCarrello(userId, "wwwroot/json/carrelli.json");
        if (carrello == null || carrello.Carrello.Count == 0)
        {
            _logger.LogWarning("Tentativo di creare un ordine con un carrello vuoto. UserId: {UserId}", userId);
            return BadRequest("Il carrello è vuoto.");
        }

        var success = _ordiniService.CreaOrdineDaCarrello(userId, carrello);
        if (!success)
        {
            return BadRequest("Errore nella creazione dell'ordine.");
        }

        _ordiniService.SvuotaCarrello(userId, "wwwroot/json/carrelli.json");

        return RedirectToAction("Index");
    }
    catch (Exception ex)
    {
        _logger.LogError($"Errore durante la creazione dell'ordine: {ex.Message}");
        return StatusCode(500, "Errore interno del server.");
    }
}

[HttpPost]
public IActionResult EliminaOrdine(int id)
{
    try
    {
        // Usa il servizio per eliminare l'ordine
        bool successo = _ordiniService.EliminaOrdine(id);

        if (!successo)
        {
            return NotFound("Ordine non trovato.");
        }

        return RedirectToAction("Index");
    }
    catch (Exception ex)
    {
        _logger.LogError("Errore durante l'eliminazione dell'ordine: {Message}", ex.Message);
        return StatusCode(500, "Errore interno del server.");
    }
}

public IActionResult DettaglioOrdine(int id)
{
    try
    {
        // Usa il servizio per ottenere il dettaglio dell'ordine
        var viewModel = _ordiniService.GetDettaglioOrdine(id);

        // Controlla se l'ordine non esiste
        if (viewModel == null)
        {
            return NotFound("Ordine non trovato.");
        }

        // Restituisce la vista con il ViewModel
        return View("DettaglioOrdini", viewModel);
    }
    catch (Exception ex)
    {
        _logger.LogError("Errore durante il caricamento del dettaglio ordine: {Message}", ex.Message);
        return StatusCode(500, "Errore interno del server.");
    }
}

}