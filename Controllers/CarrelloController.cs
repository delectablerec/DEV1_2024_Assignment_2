using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public class CarrelloController : Controller
{
    private CarrelloService _carrelloService;
    private readonly ILogger<ProdottiController> _logger;   // Logger per identificare problemi o informazioni
    private readonly UserManager<Cliente> _userManager; // Cliente eredita da IdentityUser
    private readonly ApplicationDbContext _context; // Collegamento al database
    public CarrelloController(ApplicationDbContext context, ILogger<ProdottiController> logger, UserManager<Cliente> userManager, CarrelloService carrelloService)
    {
        _context = context;
        _logger = logger;
        _userManager = userManager;
        _carrelloService = carrelloService;
    }
    public IActionResult Index()
    {
        var userId = _userManager.GetUserId(User); 
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogError("User ID is null or empty.");
            return RedirectToAction("Index", "Home"); 
        }

        var carrello = _carrelloService.CaricaCarrello(userId); // Carica il carrello

        return View(carrello);
    }

    public IActionResult AggiungiACarrello(int id)
    {
        var userId = _userManager.GetUserId(User);
        _logger.LogInformation("UserId: {UserId} is adding a product to the cart.", userId);
        
        var listaOrologi = _context.Orologi.ToList();
        if (listaOrologi == null || listaOrologi.Count == 0)
        {
            _logger.LogError("Lista vuota o nulla.");
            return NotFound();
        }
        // Passa i prodotti estrapolati dal database
        var orologio = CercaProdottoPerId(listaOrologi, id);

        if (orologio == null)
        {
            _logger.LogWarning("Product with ID: {ProductId} not found.", id);
            return NotFound(); 
        }

        _logger.LogInformation("Product found: {ProductName}, Price: {ProductPrice}", orologio.Modello, orologio.Prezzo);

        // Use the service to update the cart
        _carrelloService.AggiungiACarrello(userId, orologio);

        // Redirect to the cart view
        return RedirectToAction("Index");
    }

    private Orologio CercaProdottoPerId(List<Orologio> orologi, int id)
    {
        try
        {
            Orologio orologio = null;
            foreach (var item in orologi)
            {
                if (item.Id == id)
                {
                    orologio = item;
                    break;
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
