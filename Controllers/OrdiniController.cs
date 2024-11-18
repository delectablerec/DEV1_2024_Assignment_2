using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

public class OrdiniController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<OrdiniController> _logger;
    private const string FilePath = "wwwroot/json/carrelli.json";
    private readonly UserManager<Cliente> _userManager;

    public OrdiniController(ApplicationDbContext context, ILogger<OrdiniController> logger, UserManager<Cliente> userManager)
    {
        _context = context;
        _logger = logger;
        _userManager = userManager;
    }

public IActionResult Index()
{
    try
    {
        // Recupera gli ordini dal database
        var ordini = _context.Ordini
            .Include(o => o.Orologi) // Include la lista degli orologi associati all'ordine
            .Include(o => o.Cliente) // Include il cliente associato all'ordine
            .ToList();

        // Trasforma gli ordini in una lista di view model
        var viewModel = ordini.Select(ordine => new ListaOrdiniViewModel
        {
            Id = ordine.Id,
            NomeOrdine = ordine.Nome,
            DataAcquisto = ordine.DataAcquisto,
            StatoOrdine = ordine.Orologi.Any() ? "Completato" : "In lavorazione", // Stato dinamico basato sulla presenza di prodotti
            TotaleOrdine = ordine.Orologi.Sum(o => o.Prezzo), // Calcolo del totale
          //  UrlImmagineProdotto = ordine.Orologi.FirstOrDefault()?.UrlImmagine ?? "/img/default.png", // Immagine del primo prodotto o un'immagine di default
             UrlImmagineProdotto = ordine.Orologi.FirstOrDefault()?.UrlImmagine ?? "/img/default.png", // Immagine del primo prodotto
            NomeProdotto = ordine.Orologi.FirstOrDefault()?.Modello ?? "Nessun prodotto",
            CostoSpedizione = 10.00m // Fisso per ora
        }).ToList();

        // Passa il modello alla vista
        return View(viewModel);
    }
    catch (Exception ex)
    {
        _logger.LogError("Errore durante il caricamento degli ordini: {Message}", ex.Message);
        return StatusCode(500, "Errore interno del server.");
    }
}


[HttpPost]
public IActionResult CreaOrdineDaCarrello(string indirizzo, string metodoPagamento)
{
    var userId = _userManager.GetUserId(User); // Get the authenticated user's ID
    if (string.IsNullOrEmpty(userId))
    {
        _logger.LogWarning("Utente non autenticato. Impossibile creare un ordine.");
        return Unauthorized("Devi essere autenticato per effettuare un ordine.");
    }

    // Load the cart from the JSON file
    var carrello = CaricaCarrello(userId);
    if (carrello == null || carrello.Carrello == null || !carrello.Carrello.Any())
    {
        _logger.LogWarning("Tentativo di creare un ordine con un carrello vuoto.");
        return BadRequest("Il carrello è vuoto.");
    }

    try
    {
        var cliente = _context.Clienti.FirstOrDefault(c => c.Id == userId);
        if (cliente == null)
        {
            _logger.LogWarning("Cliente non trovato per ID: {ClienteId}.", userId);
            return NotFound("Cliente non trovato.");
        }

        // Create a new order based on the cart
        var nuovoOrdine = new Ordine
        {
            ClienteId = userId,
            Cliente = cliente,
            DataAcquisto = DateTime.Now,
            Quantita = carrello.Carrello.Sum(p => p.QuantitaInCarrello),
            Orologi = carrello.Carrello.Select(ci =>
            {
                var prodotto = ci.Orologio;

                if (_context.Orologi.Any(o => o.Id == prodotto.Id))
                {
                    _context.Orologi.Attach(prodotto);
                }
                else
                {
                    prodotto.Id = 0; // Reset ID for new products
                }

                return prodotto;
            }).ToList()
        };

        _context.Ordini.Add(nuovoOrdine);
        _context.SaveChanges();

        _logger.LogInformation("Ordine creato con successo, ID: {OrdineId}.", nuovoOrdine.Id);

        SvuotaCarrello(userId);
        
        return RedirectToAction("Index", "Home");

        return Ok($"Ordine creato con successo! ID Ordine: {nuovoOrdine.Id}");
    }
    catch (Exception ex)
    {
        _logger.LogError("Errore durante la creazione dell'ordine: {Message}", ex.Message);
        return StatusCode(500, "Errore interno al server.");
    }
}


    private CarrelloViewModel CaricaCarrello(string userId)
    {
        try
        {
            if (System.IO.File.Exists(FilePath))
            {
                var json = System.IO.File.ReadAllText(FilePath);
                var carrelliUtenti = JsonConvert.DeserializeObject<Dictionary<string, CarrelloViewModel>>(json) 
                                     ?? new Dictionary<string, CarrelloViewModel>();

                if (carrelliUtenti.TryGetValue(userId, out var carrello))
                {
                    _logger.LogInformation("Carrello caricato correttamente per UserId: {UserId}", userId);
                    return carrello;
                }
            }

            _logger.LogWarning("Carrello non trovato per UserId: {UserId}. Ritorno un carrello vuoto.", userId);
            return new CarrelloViewModel
            {
                Carrello = new List<OrologioInCarrello>(),
                Totale = 0,
                Quantita = 0
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("Errore durante il caricamento del carrello per UserId: {UserId}. Exception: {Message}", userId, ex.Message);
            throw;
        }
    }

    private void SvuotaCarrello(string userId)
    {
        try
        {
            if (System.IO.File.Exists(FilePath))
            {
                var json = System.IO.File.ReadAllText(FilePath);
                var carrelliUtenti = JsonConvert.DeserializeObject<Dictionary<string, CarrelloViewModel>>(json)
                                     ?? new Dictionary<string, CarrelloViewModel>();

                if (carrelliUtenti.ContainsKey(userId))
                {
                    carrelliUtenti[userId] = new CarrelloViewModel
                    {
                        Carrello = new List<OrologioInCarrello>(),
                        Totale = 0,
                        Quantita = 0
                    };

                    var updatedJson = JsonConvert.SerializeObject(carrelliUtenti, Formatting.Indented);
                    System.IO.File.WriteAllText(FilePath, updatedJson);

                    _logger.LogInformation("Carrello svuotato per UserId: {UserId}", userId);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Errore durante lo svuotamento del carrello per UserId: {UserId}. Exception: {Message}", userId, ex.Message);
        }
    }

[HttpPost]
public IActionResult EliminaOrdine(int id)
{
    try
    {
        var ordine = _context.Ordini.Include(o => o.Orologi).FirstOrDefault(o => o.Id == id);
        if (ordine == null)
        {
            _logger.LogWarning("Ordine non trovato con ID: {Id}", id);
            return NotFound("Ordine non trovato.");
        }

        // Disassocia i prodotti dall'ordine
        ordine.Orologi.Clear();

        // Rimuovi l'ordine
        _context.Ordini.Remove(ordine);
        _context.SaveChanges();

        _logger.LogInformation("Ordine con ID {Id} eliminato con successo.", id);

        // Dopo l'eliminazione, reindirizza alla vista degli ordini
        return RedirectToAction("Index");
    }
    catch (Exception ex)
    {
        _logger.LogError("Errore durante l'eliminazione dell'ordine: {Message}", ex.Message);
        return StatusCode(500, "Errore interno al server.");
    }
}
/*metodo funzionante prima di mettere il totale ordine
[HttpGet]
public IActionResult DettaglioOrdine(int id)
{
    try
    {
        // Recupera l'ordine dal database includendo i prodotti e il cliente associato
        var ordine = _context.Ordini
            .Include(o => o.Orologi)
            .Include(o => o.Cliente)
            .FirstOrDefault(o => o.Id == id);

        if (ordine == null)
        {
            _logger.LogWarning("Ordine non trovato con ID: {Id}", id);
            return NotFound("Ordine non trovato.");
        }

        // Popola il view model con i dettagli dell'ordine
        var viewModel = new DettaglioOrdineViewModel
        {
            OrdineId = ordine.Id,
            NomeOrdine = ordine.Nome,
            ClienteNome = ordine.Cliente.Nome,
            ClienteTelefono = "0000000",
            IndirizzoSpedizione = "Unknown street",
            MetodoPagamento = "Carta di credito", // Modifica in base alla tua logica
            TipoSpedizione = "Standard",         // Modifica in base alla tua logica
            CostoSpedizione = 10.00m,            // Default per ora
            StatoOrdine = "Completato",          // Modifica in base allo stato reale
            DataAcquisto = ordine.DataAcquisto,
            Prodotti = ordine.Orologi.ToList()
        };

        return View("DettaglioOrdini",viewModel);
    }
    catch (Exception ex)
    {
        _logger.LogError("Errore durante il caricamento del dettaglio ordine: {Message}", ex.Message);
        return StatusCode(500, "Errore interno al server.");
    }
}*/


[HttpGet]
public IActionResult DettaglioOrdine(int id)
{
    try
    {
        // Recupera l'ordine dal database includendo i prodotti e il cliente associato
        var ordine = _context.Ordini
            .Include(o => o.Orologi) // Include i prodotti associati
            .Include(o => o.Cliente) // Include il cliente associato
            .FirstOrDefault(o => o.Id == id);

        if (ordine == null)
        {
            _logger.LogWarning("Ordine non trovato con ID: {Id}", id);
            return NotFound("Ordine non trovato.");
        }

        // Calcola il subtotale
        decimal subtotale = ordine.Orologi.Sum(p => p.Prezzo);

        decimal costoSpedizione = 10.00m;

        // Calcola il totale
        decimal totale = subtotale + costoSpedizione;

        // Calcola il totale dell'ordine
     //   var totaleOrdine = ordine.Orologi.Sum(p => p.Prezzo * p.Giacenza);

        // Popola il view model con i dettagli dell'ordine
        var viewModel = new DettaglioOrdineViewModel
        {
            OrdineId = ordine.Id,
            NomeOrdine = ordine.Nome,
            ClienteNome = ordine.Cliente.Nome,
        //    ClienteTelefono = ordine.Cliente.PhoneNumber ?? "Non disponibile", // Usa un valore predefinito se null
           // IndirizzoSpedizione = ordine.Cliente.Indirizzo ?? "Indirizzo non disponibile", // Usa un valore predefinito se null
            MetodoPagamento = "Carta di credito", // Modifica in base alla tua logica
            TipoSpedizione = "Standard",         // Modifica in base alla tua logica
            CostoSpedizione = costoSpedizione,            // Default 
            StatoOrdine = "Completato",          // Modifica in base allo stato reale
            DataAcquisto = ordine.DataAcquisto,
            Prodotti = ordine.Orologi.ToList(),
            // Totale del carrello (ordine)
            Subtotale = subtotale,
            Totale = totale
        };

        return View("DettaglioOrdini", viewModel);
    }
    catch (Exception ex)
    {
        _logger.LogError("Errore durante il caricamento del dettaglio ordine: {Message}", ex.Message);
        return StatusCode(500, "Errore interno al server.");
    }
}




}
