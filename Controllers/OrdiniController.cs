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

    // Metodo per visualizzare tutti gli ordini
    public IActionResult Index()
    {
        try
        {
            var ordini = _context.Ordini
                .Include(o => o.OrdineDettagli)
                    .ThenInclude(od => od.Orologio)
                .Include(o => o.Cliente)
                .ToList();

            var viewModel = new List<ListaOrdiniViewModel>();
            foreach (var ordine in ordini)
            {
                var totaleOrdine = 0m;
                foreach (var dettaglio in ordine.OrdineDettagli)
                {
                    totaleOrdine += dettaglio.PrezzoUnitario * dettaglio.Quantita;
                }

                viewModel.Add(new ListaOrdiniViewModel
                {
                    Id = ordine.Id,
                    NomeOrdine = ordine.Nome,
                    DataAcquisto = ordine.DataAcquisto,
                    StatoOrdine = ordine.OrdineDettagli.Count > 0 ? "Completato" : "In lavorazione",
                    TotaleOrdine = totaleOrdine,
                    UrlImmagineProdotto = ordine.OrdineDettagli.Count > 0
                        ? ordine.OrdineDettagli[0].Orologio.UrlImmagine
                        : "/img/default.png",
                    NomeProdotto = ordine.OrdineDettagli.Count > 0
                        ? ordine.OrdineDettagli[0].Orologio.Modello
                        : "Nessun prodotto",
                    CostoSpedizione = 10.00m
                });
            }

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError("Errore durante il caricamento degli ordini: {Message}", ex.Message);
            return StatusCode(500, "Errore interno del server.");
        }
    }

 

    // Metodo per caricare il carrello dal file JSON
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
        catch (Exception ex)
        {
            _logger.LogError("Errore durante il caricamento del carrello per UserId: {UserId}. Exception: {Message}", userId, ex.Message);
            throw;
        }
    }

    // Metodo per svuotare il carrello
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
/*
    // Metodo per creare un ordine a partire dal carrello
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
            if (carrello == null || carrello.Carrello.Count == 0)
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

            var nuovoOrdine = new Ordine
            {
                ClienteId = userId,
                Cliente = cliente,
                DataAcquisto = DateTime.Now,
                Nome = $"Ordine-{DateTime.Now.Ticks}_{userId}"
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

            nuovoOrdine.Nome = $"BRT-{nuovoOrdine.Id}_{userId}";
            _context.SaveChanges();

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
*/

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
        if (carrello == null || carrello.Carrello.Count == 0)
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

        var nuovoOrdine = new Ordine
        {
            ClienteId = userId,
            Cliente = cliente,
            DataAcquisto = DateTime.Now,
            Nome = $"Ordine-{DateTime.Now.Ticks}_{userId}"
        };

        foreach (var item in carrello.Carrello)
        {
            var prodotto = _context.Orologi.FirstOrDefault(p => p.Id == item.Orologio.Id);
            if (prodotto == null)
            {
                _logger.LogWarning("Prodotto con ID {IdProdotto} non trovato.", item.Orologio.Id);
                continue;
            }

            // Non alteriamo la giacenza del prodotto in questa fase
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

        nuovoOrdine.Nome = $"BRT-{nuovoOrdine.Id}_{userId}";
        _context.SaveChanges();

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

 
    public IActionResult DettaglioOrdine(int id)
{
    try
    {
        // Recupera tutti gli ordini dal contesto, includendo i dettagli e i clienti
        List<Ordine> ordini = _context.Ordini
            .Include("OrdineDettagli.Orologio")
            .Include("Cliente")
            .ToList();

        // Trova l'ordine con l'ID specificato
        Ordine ordine = null;
        foreach (var o in ordini)
        {
            if (o.Id == id)
            {
                ordine = o;
                break;
            }
        }

        // Controlla se l'ordine esiste
        if (ordine == null)
        {
            _logger.LogWarning("Ordine non trovato con ID: {Id}", id);
            return NotFound("Ordine non trovato.");
        }

        // Crea il ViewModel per il dettaglio ordine
        DettaglioOrdineViewModel viewModel = new DettaglioOrdineViewModel
        {
            OrdineId = ordine.Id,
            NomeOrdine = ordine.Nome,
            ClienteNome = ordine.Cliente != null ? ordine.Cliente.Nome : "Cliente sconosciuto",
            IndirizzoSpedizione = "Via Esempio, 123", // Valore statico
            MetodoPagamento = "Carta di credito",     // Valore statico
            TipoSpedizione = "Standard",             // Valore statico
            StatoOrdine = ordine.OrdineDettagli.Count > 0 ? "Completato" : "In lavorazione",
            DataAcquisto = ordine.DataAcquisto,
            Subtotale = 0m,
            CostoSpedizione = 10.00m, // Valore fisso
            Totale = 0m,
            Prodotti = new List<DettaglioOrdineProdottoViewModel>()
        };

        // Calcola il totale e aggiungi i dettagli del prodotto al ViewModel
        foreach (var dettaglio in ordine.OrdineDettagli)
        {
            var prezzoTotaleDettaglio = dettaglio.PrezzoUnitario * dettaglio.Quantita;
            viewModel.Subtotale += prezzoTotaleDettaglio;

            // Aggiungi il prodotto al ViewModel
            DettaglioOrdineProdottoViewModel prodottoViewModel = new DettaglioOrdineProdottoViewModel
            {
                UrlImmagine = dettaglio.Orologio != null ? dettaglio.Orologio.UrlImmagine : "/img/default.png",
                Modello = dettaglio.Orologio != null ? dettaglio.Orologio.Modello : "Modello sconosciuto",
                Quantita = dettaglio.Quantita,
                PrezzoUnitario = dettaglio.PrezzoUnitario,
                Descrizione = $"Quantità: {dettaglio.Quantita} - Prezzo unitario: €{dettaglio.PrezzoUnitario}",
                Giacenza = dettaglio.Orologio != null ? dettaglio.Orologio.Giacenza : 0
            };

            viewModel.Prodotti.Add(prodottoViewModel);
        }

        // Calcola il totale dell'ordine
        viewModel.Totale = viewModel.Subtotale + viewModel.CostoSpedizione;

        // Restituisce la vista con il ViewModel
        return View("DettaglioOrdini", viewModel);
    }
    catch (Exception ex)
    {
        _logger.LogError("Errore durante il caricamento del dettaglio ordine: {Message}", ex.Message);
        return StatusCode(500, "Errore interno del server.");
    }
}


[HttpPost]
public IActionResult EliminaOrdine(int id)
{
    try
    {
        // Recupera l'ordine dal database, inclusi i dettagli
        Ordine ordine = null;
        foreach (var o in _context.Ordini.Include("OrdineDettagli.Orologio"))
        {
            if (o.Id == id)
            {
                ordine = o;
                break;
            }
        }

        // Controlla se l'ordine esiste
        if (ordine == null)
        {
            _logger.LogWarning("Ordine non trovato con ID: {Id}", id);
            return NotFound("Ordine non trovato.");
        }

        // Aggiorna la giacenza per ogni prodotto nell'ordine
        foreach (var dettaglio in ordine.OrdineDettagli.ToList())
        {
            if (dettaglio.Orologio != null)
            {
                dettaglio.Orologio.Giacenza += dettaglio.Quantita;
                _context.Entry(dettaglio.Orologio).State = EntityState.Modified;
            }

            // Rimuovi il dettaglio dell'ordine
            _context.Entry(dettaglio).State = EntityState.Deleted;
        }

        // Rimuovi l'ordine
        _context.Ordini.Remove(ordine);

        // Salva le modifiche al database
        _context.SaveChanges();

        _logger.LogInformation("Ordine con ID {Id} eliminato con successo.", id);

        // Reindirizza alla lista degli ordini
        return RedirectToAction("Index");
    }
    catch (Exception ex)
    {
        _logger.LogError("Errore durante l'eliminazione dell'ordine: {Message}", ex.Message);
        return StatusCode(500, "Errore interno del server.");
    }
}



}
