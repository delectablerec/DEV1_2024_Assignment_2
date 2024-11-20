using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public class CarrelloController : Controller
{
    private readonly ILogger<ProdottiController> _logger;   // Logger per identificare problemi o informazioni
    private readonly UserManager<Cliente> _userManager; // Cliente eredita da IdentityUser
    private CarrelloService _carrelloService;
    private readonly ApplicationDbContext _context; // Collegamento al database
    public CarrelloController(ApplicationDbContext context, ILogger<ProdottiController> logger, UserManager<Cliente> userManager, CarrelloService carrelloService)
    {
        _context = context;
        _logger = logger;
        _userManager = userManager;
        _carrelloService = carrelloService;
    }
    /*
        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User); 
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogError("User ID is null or empty.");
                return RedirectToAction("Index", "Home"); 
            }

            // Recupera o inizializza il carrello per l'utente tramite il servizio
            var carrello = _carrelloService.CaricaCarrello(userId);

            // Passa il carrello alla view
            return View(carrello);
        }*/

    public IActionResult Index()
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogError("User ID is null or empty.");
            return RedirectToAction("Index", "Home");
        }

        var carrello = _carrelloService.CaricaCarrello(userId);

        if (carrello == null || carrello.Carrello.Count == 0)
        {
            _logger.LogWarning("Carrello vuoto per UserId: {UserId}", userId);
        }
        else
        {
            _logger.LogInformation("Carrello caricato per UserId: {UserId}. Prodotti nel carrello: {Count}", userId, carrello.Carrello.Count);
        }
        
        ViewData["CartItemCount"] = _carrelloService.ItemsInCart(User);
        return View(carrello);
    }


    public IActionResult AggiungiACarrello(int id)
    {
        var userId = _userManager.GetUserId(User);
        _logger.LogInformation("UserId: {userId} sta aggiungendo un prodotto al carrello.", userId);

        var listaOrologi = _context.Orologi.ToList();
        if (listaOrologi == null || listaOrologi.Count == 0)
        {
            _logger.LogError("Lista vuota o nulla");
            return NotFound();
        }

        var orologio = _carrelloService.CercaProdottoPerId(listaOrologi, id);
        if (orologio == null)
        {
            _logger.LogWarning("Prodotto con ID: {IdProdotto} non trovato", id);
            return NotFound();
        }

        _logger.LogInformation("Prodotto trovato: {NomeProdotto}, Prezzo: {PrezzoProdotto}", orologio.Modello, orologio.Prezzo);

        // Usa il servizio per aggiornare il carrello
        var success = _carrelloService.AggiungiACarrello(userId, orologio);
        if (!success)
        {
            _logger.LogWarning("Giacenza del prodotto insufficiente {IdProdotto}", id);
            return RedirectToAction("Index", "Prodotti");
        }

        return RedirectToAction("Index");
    }
    public IActionResult RimuoviUnoDalCarrello(int id)
    {
        var userId = _userManager.GetUserId(User);
        _logger.LogInformation("UserId: {userId} sta rimuovendo un prodotto dal carrello.", userId);

        var listaOrologi = _context.Orologi.ToList();
        if (listaOrologi == null || listaOrologi.Count == 0)
        {
            _logger.LogError("Lista vuota o nulla");
            return NotFound();
        }

        var orologio = _carrelloService.CercaProdottoPerId(listaOrologi, id);
        if (orologio == null)
        {
            _logger.LogWarning("Prodotto con ID: {IdProdotto} non trovato", id);
            return NotFound();
        }

        _logger.LogInformation("Prodotto trovato: {NomeProdotto}, Prezzo: {PrezzoProdotto}", orologio.Modello, orologio.Prezzo);

        // Usa il servizio per aggiornare il carrello
        _carrelloService.RimuoviUnoDalCarrello(userId, orologio);

        return RedirectToAction("Index");
    }

    public IActionResult RimuoviDalCarrello(int id)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogError("User ID è nullo o vuoto.");
            return RedirectToAction("Index", "Home");
        }

        // Chiama il metodo del servizio per rimuovere dal carrello
        var success = _carrelloService.RimuoviDalCarrello(userId, id);
        if (!success)
        {
            _logger.LogWarning("Errore nella rimozione del prodotto con ID: {IdProdotto} dal carrello", id);
            return RedirectToAction("Index");
        }

        return RedirectToAction("Index"); // Redirect al carrello dopo la rimozione
    }
}
