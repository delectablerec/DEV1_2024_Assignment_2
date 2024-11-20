using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;


// Controller per gestire le operazioni sugli ordini
public class OrdiniController : Controller
{
  
// Costruttore del controller


    
    private CarrelloService _carrelloService;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<OrdiniController> _logger;
    private readonly OrdiniService _ordiniService;  // Servizio per la logica di gestione degli ordini
    private readonly UserManager<Cliente> _userManager; // Gestore per l'identità degli utenti
    private const string FilePath = "wwwroot/json/carrelli.json";

    public OrdiniController(ApplicationDbContext context, ILogger<OrdiniController> logger, UserManager<Cliente> userManager, CarrelloService carrelloService, OrdiniService ordiniService)
    {
        _context = context;
        _logger = logger;
        _userManager = userManager;
        _carrelloService = carrelloService;
        _ordiniService = ordiniService;
    }


// Controller per gestire le operazioni sugli ordini


// Azione per visualizzare la lista degli ordini
    public IActionResult Index()
    {
        try
        {
            var ordini = _ordiniService.GetOrdini(); // Recupera la lista degli ordini tramite il servizio
            // Restituisci la vista con il ViewModel
            ViewData["CartItemCount"] = _carrelloService.ItemsInCart(User);
            return View(ordini);
        }
        catch
        {
            return StatusCode(500, "Errore interno del server.");
        }
    }

// Azione per creare un ordine dal carrello di un utente autenticato
    [HttpPost]
public IActionResult CreaOrdineDaCarrello()
{
    try
    {
        var userId = _userManager.GetUserId(User); // Recupera l'ID dell'utente autenticato
        if (string.IsNullOrEmpty(userId)) //se non è autenticato
        {
            _logger.LogWarning("Utente non autenticato. Impossibile creare un ordine.");
            return Unauthorized("Devi essere autenticato per effettuare un ordine.");
        }
    // Carica il carrello dal file JSON
    //Il metodo CaricaCarrello utilizza l'userId per cercare nel file JSON il carrello associato a quell'utente
        var carrello = _ordiniService.CaricaCarrello(userId, "wwwroot/json/carrelli.json");
        if (carrello == null || carrello.Carrello.Count == 0)
        {
            _logger.LogWarning("Tentativo di creare un ordine con un carrello vuoto. UserId: {UserId}", userId);
            return BadRequest("Il carrello è vuoto.");
        }
    //  creare un nuovo ordine basato sui dati presenti nel carrello dell'utente specifico
        var success = _ordiniService.CreaOrdineDaCarrello(userId, carrello); 
        if (!success) //se l'ordine non è stato creato
        {
            return BadRequest("Errore nella creazione dell'ordine.");
        }

        _ordiniService.SvuotaCarrello(userId, "wwwroot/json/carrelli.json"); // Svuota il carrello una volta creato l'ordine

        return RedirectToAction("Index");
    }
    catch (Exception ex)
    {
        _logger.LogError($"Errore durante la creazione dell'ordine: {ex.Message}");
        return StatusCode(500, "Errore interno del server.");
    }
}

// Azione per eliminare un ordine
[HttpPost]
public IActionResult EliminaOrdine(int id)
{
    try
    {
        //Chiama il metodo EliminaOrdine del servizio OrdiniService passando l'ID dell'ordine da eliminare.
        //Il metodo EliminaOrdine restituisce un valore booleano true se è andata a buon fine
        bool successo = _ordiniService.EliminaOrdine(id);

        if (!successo) // Controlla se l'ordine non è stato trovato
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

// Azione per visualizzare il dettaglio di un ordine
public IActionResult DettaglioOrdine(int id)
{
    try
    {
       // richiama il metodo che cerca l'ordine con l'ID specificato nel db includendo i dettagli dell'ordine e i dati del cliente
        var viewModel = _ordiniService.GetDettaglioOrdine(id);

        // Controlla se l'ordine non esiste
        if (viewModel == null)
        {
            return NotFound("Ordine non trovato.");
        }

        // Restituisce la vista con il ViewModel aggiornato
        return View("DettaglioOrdini", viewModel);
    }
    catch (Exception ex)
    {
        _logger.LogError("Errore durante il caricamento del dettaglio ordine: {Message}", ex.Message);
        return StatusCode(500, "Errore interno del server.");
    }
}

}