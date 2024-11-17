using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

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

    // Visualizza l'elenco degli ordini
    public IActionResult Index()
    {
        try
        {
            _logger.LogInformation("Caricamento della lista degli ordini.");

            var ordini = _context.Ordini
                .Include(o => o.Orologi)
                .Include(o => o.Cliente)
                .ToList();

            var viewModel = ordini.Select(ordine => new ListaOrdiniViewModel
            {
                Id = ordine.Id,
                NomeOrdine = ordine.Nome,
                DataAcquisto = ordine.DataAcquisto,
                StatoOrdine = ordine.StatoOrdine.ToString(),
                TotaleOrdine = ordine.Orologi.Sum(o => o.Prezzo),
                UrlImmagineProdotto = ordine.Orologi.FirstOrDefault()?.UrlImmagine,
                NomeProdotto = ordine.Orologi.FirstOrDefault()?.Modello,
                CostoSpedizione = ordine.CostoSpedizione
            }).ToList();

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError("Errore durante il caricamento della lista degli ordini: {Message}", ex.Message);
            return StatusCode(500, "Errore interno al server.");
        }
    }

    [HttpPost]
    public IActionResult CreaOrdineDaCarrello(CarrelloViewModel carrello, string indirizzo, string metodoPagamento)
    {
        if (carrello == null || carrello.Carrello == null || !carrello.Carrello.Any())
        {
            _logger.LogWarning("Tentativo di creare un ordine con un carrello vuoto.");
            return BadRequest("Il carrello è vuoto.");
        }

        try
        {
            var clienteId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(clienteId))
            {
                _logger.LogWarning("Utente non autenticato. Impossibile creare un ordine.");
                return Unauthorized("Devi essere autenticato per effettuare un ordine.");
            }

            var cliente = _context.Clienti.FirstOrDefault(c => c.Id == clienteId);
            if (cliente == null)
            {
                _logger.LogWarning("Cliente non trovato per ID: {ClienteId}.", clienteId);
                return NotFound("Cliente non trovato.");
            }

            _logger.LogInformation("Creazione ordine in corso per il cliente con ID: {ClienteId}.", clienteId);

            var nuovoOrdine = new Ordine
            {
                ClienteId = clienteId,
                Cliente = cliente,
                DataAcquisto = DateTime.Now,
                Quantita = carrello.Carrello.Sum(p => p.QuantitaInCarrello),
                MetodoPagamento = metodoPagamento,
                IndirizzoSpedizione = indirizzo,
                Orologi = carrello.Carrello.Select(ci => ci.Orologio).ToList(),
                CostoSpedizione = 10.00m,
                StatoOrdine = StatoOrdine.InLavorazione
            };

            _context.Ordini.Add(nuovoOrdine);
            _context.SaveChanges();

            _logger.LogInformation("Ordine creato con successo, ID: {OrdineId}.", nuovoOrdine.Id);

            SvuotaCarrello(clienteId);

            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError("Errore durante la creazione dell'ordine: {Message}", ex.Message);
            return StatusCode(500, "Errore interno al server.");
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
}

