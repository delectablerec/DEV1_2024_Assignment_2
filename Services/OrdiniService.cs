using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

public class OrdiniService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<OrdiniService> _logger;

    public OrdiniService(ApplicationDbContext context, ILogger<OrdiniService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public List<ListaOrdiniViewModel> GetOrdini()
    {
        try
        {
            var ordini = _context.Ordini
                .Include("OrdineDettagli.Orologio")
                .Include("Cliente")
                .ToList();

            return ordini.Select(ordine =>
            {
                var totaleOrdine = ordine.OrdineDettagli.Sum(d => d.PrezzoUnitario * d.Quantita);
                var statoOrdine = ordine.OrdineDettagli.Count > 0 ? "Completato" : "In lavorazione";

                var urlImmagineProdotto = ordine.OrdineDettagli.Count > 0 && ordine.OrdineDettagli[0].Orologio != null
                    ? ordine.OrdineDettagli[0].Orologio.UrlImmagine
                    : "/img/default.png";

                var nomeProdotto = ordine.OrdineDettagli.Count > 0 && ordine.OrdineDettagli[0].Orologio != null
                    ? ordine.OrdineDettagli[0].Orologio.Modello
                    : "Nessun prodotto";

                return new ListaOrdiniViewModel
                {
                    Id = ordine.Id,
                    NomeOrdine = ordine.Nome,
                    DataAcquisto = ordine.DataAcquisto,
                    StatoOrdine = statoOrdine,
                    TotaleOrdine = totaleOrdine,
                    UrlImmagineProdotto = urlImmagineProdotto,
                    NomeProdotto = nomeProdotto,
                    CostoSpedizione = 10.00m
                };
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Errore durante il recupero degli ordini: {ex.Message}");
            throw;
        }
    }

  public void SvuotaCarrello(string userId, string filePath)
{
    try
    {
        if (System.IO.File.Exists(filePath))
        {
            var json = System.IO.File.ReadAllText(filePath);
            var carrelliUtenti = JsonConvert.DeserializeObject<Dictionary<string, CarrelloViewModel>>(json) ??
                                 new Dictionary<string, CarrelloViewModel>();

            if (carrelliUtenti.ContainsKey(userId))
            {
                carrelliUtenti[userId] = new CarrelloViewModel
                {
                    Carrello = new List<OrologioInCarrello>(),
                    Totale = 0,
                    Quantita = 0
                };

                var updatedJson = JsonConvert.SerializeObject(carrelliUtenti, Formatting.Indented);
                System.IO.File.WriteAllText(filePath, updatedJson);
                _logger.LogInformation("Carrello svuotato per UserId: {UserId}", userId);
            }
        }
    }
    catch (Exception ex)
    {
        _logger.LogError($"Errore durante lo svuotamento del carrello: {ex.Message}");
        throw;
    }
}


    public CarrelloViewModel CaricaCarrello(string userId, string filePath)
    {
        try
        {
            if (System.IO.File.Exists(filePath))
            {
                var json = System.IO.File.ReadAllText(filePath);

                var carrelliUtenti = JsonConvert.DeserializeObject<Dictionary<string, CarrelloViewModel>>(json) ??
                                     new Dictionary<string, CarrelloViewModel>();

                if (carrelliUtenti.TryGetValue(userId, out var carrello))
                {
                    return carrello;
                }
            }

            return new CarrelloViewModel
            {
                Carrello = new List<OrologioInCarrello>(),
                Totale = 0,
                Quantita = 0
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Errore durante il caricamento del carrello: {ex.Message}");
            throw;
        }
    }

    public bool CreaOrdineDaCarrello(string userId, CarrelloViewModel carrello)
    {
        try
        {
            var cliente = _context.Clienti.FirstOrDefault(c => c.Id == userId);
            if (cliente == null) return false;

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
                if (prodotto == null) continue;

                nuovoOrdine.OrdineDettagli.Add(new OrdineDettaglio
                {
                    Ordine = nuovoOrdine,
                    Orologio = prodotto,
                    Quantita = item.QuantitaInCarrello,
                    PrezzoUnitario = prodotto.Prezzo
                });
            }

            _context.Ordini.Add(nuovoOrdine);
            _context.SaveChanges();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Errore durante la creazione dell'ordine: {ex.Message}");
            return false;
        }
    }

     public bool EliminaOrdine(int id)
    {
        try
        {
            Ordine ordineDaEliminare = null;

            // Ricerca dell'ordine
            foreach (var ordine in _context.Ordini.Include("OrdineDettagli.Orologio"))
            {
                if (ordine.Id == id)
                {
                    ordineDaEliminare = ordine;
                    break;
                }
            }

            // Se l'ordine non esiste
            if (ordineDaEliminare == null)
            {
                _logger.LogWarning("Ordine con ID {Id} non trovato.", id);
                return false;
            }

            // Aggiorna la giacenza e rimuove i dettagli
            foreach (var dettaglio in ordineDaEliminare.OrdineDettagli)
            {
                if (dettaglio.Orologio != null)
                {
                    dettaglio.Orologio.Giacenza += dettaglio.Quantita;
                    _context.Entry(dettaglio.Orologio).State = EntityState.Modified;
                }
                _context.Entry(dettaglio).State = EntityState.Deleted;
            }

            // Rimuove l'ordine
            _context.Ordini.Remove(ordineDaEliminare);

            // Salva le modifiche
            _context.SaveChanges();

            _logger.LogInformation("Ordine con ID {Id} eliminato con successo.", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError("Errore durante l'eliminazione dell'ordine: {Message}", ex.Message);
            throw;
        }
    }

    public DettaglioOrdineViewModel GetDettaglioOrdine(int id)
    {
        try
        {
            // Recupera tutti gli ordini dal contesto, inclusi i dettagli e i clienti
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
                _logger.LogWarning("Ordine con ID {Id} non trovato.", id);
                return null;
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

            return viewModel;
        }
        catch (Exception ex)
        {
            _logger.LogError("Errore durante il caricamento del dettaglio ordine: {Message}", ex.Message);
            throw;
        }
    }
}


