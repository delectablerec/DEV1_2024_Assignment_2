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
            var ordini = _context.Ordini
                .Include(o => o.OrdineDettagli)
                    .ThenInclude(od => od.Orologio)
                .Include(o => o.Cliente)
                .ToList();

            var viewModel = ordini.Select(ordine => new ListaOrdiniViewModel
            {
                Id = ordine.Id,
                NomeOrdine = ordine.Nome,
                DataAcquisto = ordine.DataAcquisto,
                StatoOrdine = ordine.OrdineDettagli.Any() ? "Completato" : "In lavorazione",
                TotaleOrdine = ordine.OrdineDettagli.Sum(od => od.PrezzoUnitario * od.Quantita),
                UrlImmagineProdotto = ordine.OrdineDettagli.FirstOrDefault()?.Orologio.UrlImmagine ?? "/img/default.png",
                NomeProdotto = ordine.OrdineDettagli.FirstOrDefault()?.Orologio.Modello ?? "Nessun prodotto",
                CostoSpedizione = 10.00m
            }).ToList();

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError("Errore durante il caricamento degli ordini: {Message}", ex.Message);
            return StatusCode(500, "Errore interno del server.");
        }
    }

    private CarrelloViewModel CaricaCarrello(string userId)
    {
        try
        {
            if (System.IO.File.Exists(FilePath))
            {
                var json = System.IO.File.ReadAllText(FilePath);
                _logger.LogInformation("File JSON caricato correttamente.");

                var carrelliUtenti = JsonConvert.DeserializeObject<Dictionary<string, CarrelloViewModel>>(json,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    }) ?? new Dictionary<string, CarrelloViewModel>();

                if (carrelliUtenti.TryGetValue(userId, out var carrello))
                {
                    _logger.LogInformation("Carrello trovato per UserId: {UserId}. Prodotti nel carrello: {Count}", userId, carrello.Carrello.Count);
                    return carrello;
                }
                else
                {
                    _logger.LogWarning("Carrello non trovato per UserId: {UserId}.", userId);
                }
            }
            else
            {
                _logger.LogWarning("File JSON non trovato: {FilePath}", FilePath);
            }

            // Restituisci un carrello vuoto
            return new CarrelloViewModel
            {
                Carrello = new List<OrologioInCarrello>(),
                Totale = 0,
                Quantita = 0
            };
        }
        catch (JsonSerializationException ex)
        {
            _logger.LogError("Errore di deserializzazione per UserId: {UserId}. Exception: {Message}", userId, ex.Message);
            throw;
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

        var carrello = CaricaCarrello(userId);
        if (carrello == null || !carrello.Carrello.Any())
        {
            _logger.LogWarning("Tentativo di creare un ordine con un carrello vuoto. UserId: {UserId}", userId);
            return BadRequest("Il carrello è vuoto.");
        }

        var cliente = _context.Clienti.FirstOrDefault(c => c.Id == userId);
        if (cliente == null)
        {
            _logger.LogWarning("Cliente non trovato. UserId: {UserId}", userId);
            return BadRequest("Cliente non trovato.");
        }

        // Creazione di un nuovo ordine
        var nuovoOrdine = new Ordine
        {
            ClienteId = userId,
            Cliente = cliente,
            DataAcquisto = DateTime.Now,
            Nome = $"Ordine-{DateTime.Now.Ticks}_{userId}" // Genera un nome temporaneo
        };

        foreach (var item in carrello.Carrello)
        {
            var prodotto = _context.Orologi.FirstOrDefault(p => p.Id == item.Orologio.Id);
            if (prodotto == null)
            {
                _logger.LogWarning("Prodotto con ID {IdProdotto} non trovato.", item.Orologio.Id);
                continue;
            }

            if (prodotto.Giacenza < item.QuantitaInCarrello)
            {
                throw new Exception($"Giacenza insufficiente per il prodotto {prodotto.Modello}.");
            }

            prodotto.Giacenza -= item.QuantitaInCarrello;

            var dettaglio = new OrdineDettaglio
            {
                Ordine = nuovoOrdine,
                Orologio = prodotto,
                Quantita = item.QuantitaInCarrello,
                PrezzoUnitario = prodotto.Prezzo
            };

            nuovoOrdine.OrdineDettagli.Add(dettaglio);
        }

        _context.Ordini.Add(nuovoOrdine);
        _context.SaveChanges();

        // Aggiornamento del Nome con l'ID generato
        nuovoOrdine.Nome = $"BRT-{nuovoOrdine.Id}_{userId}";
        _context.SaveChanges(); // Salva l'aggiornamento del nome

        _logger.LogInformation("Ordine creato con successo, ID: {OrdineId}.", nuovoOrdine.Id);

        SvuotaCarrello(userId);

        return RedirectToAction("Index", "Ordini");
    }
    catch (Exception ex)
    {
        _logger.LogError("Errore durante la creazione dell'ordine: {Message}", ex.Message);
        return StatusCode(500, "Errore interno del server.");
    }
}

    [HttpPost]
    public IActionResult EliminaOrdine(int id)
    {
        try
        {
            var ordine = _context.Ordini
                .Include(o => o.OrdineDettagli)
                .FirstOrDefault(o => o.Id == id);

            if (ordine == null)
            {
                _logger.LogWarning("Ordine non trovato con ID: {Id}", id);
                return NotFound("Ordine non trovato.");
            }

            _context.Ordini.Remove(ordine);
            _context.SaveChanges();

            _logger.LogInformation("Ordine con ID {Id} eliminato con successo.", id);

            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError("Errore durante l'eliminazione dell'ordine: {Message}", ex.Message);
            return StatusCode(500, "Errore interno del server.");
        }
    }



[HttpGet]
public IActionResult DettaglioOrdine(int id)
{
    try
    {
        // Recupera l'ordine dal database includendo i dettagli dell'ordine e il cliente
        var ordine = _context.Ordini
            .Include(o => o.OrdineDettagli)
                .ThenInclude(od => od.Orologio)
            .Include(o => o.Cliente)
            .FirstOrDefault(o => o.Id == id);

        if (ordine == null)
        {
            _logger.LogWarning("Ordine non trovato con ID: {Id}", id);
            return NotFound("Ordine non trovato.");
        }

        // Creazione del view model per il dettaglio ordine
        var viewModel = new DettaglioOrdineViewModel
        {
            OrdineId = ordine.Id,
            NomeOrdine = ordine.Nome,
            ClienteNome = ordine.Cliente.Nome,
            IndirizzoSpedizione = "Via Esempio, 123", 
            MetodoPagamento = "Carta di credito", 
            TipoSpedizione = "Standard",         
            StatoOrdine = ordine.OrdineDettagli.Any() ? "Completato" : "In lavorazione",
            DataAcquisto = ordine.DataAcquisto,
            Subtotale = ordine.OrdineDettagli.Sum(od => od.PrezzoUnitario * od.Quantita),
            CostoSpedizione = 10.00m, // Valore fisso, può essere calcolato dinamicamente
            Totale = ordine.OrdineDettagli.Sum(od => od.PrezzoUnitario * od.Quantita) + 10.00m,
            Prodotti = ordine.OrdineDettagli.Select(od => new DettaglioOrdineProdottoViewModel
            {
                UrlImmagine = od.Orologio.UrlImmagine,
                Modello = od.Orologio.Modello,
                Quantita = od.Quantita,
                PrezzoUnitario = od.PrezzoUnitario,
                Descrizione = $"Quantità: {od.Quantita} - Prezzo unitario: €{od.PrezzoUnitario}",
                Giacenza = od.Orologio.Giacenza
            }).ToList()
        };

        return View("DettaglioOrdini", viewModel);
    }
    catch (Exception ex)
    {
        _logger.LogError("Errore durante il caricamento del dettaglio ordine: {Message}", ex.Message);
        return StatusCode(500, "Errore interno del server.");
    }
}


}
