using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

// Classe per gestire la logica degli ordini
public class OrdiniService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<OrdiniService> _logger;

    public OrdiniService(ApplicationDbContext context, ILogger<OrdiniService> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Metodo per ottenere la lista degli ordini
    public List<ListaOrdiniViewModel> GetOrdini()
    {
        try
        {
            var ordini = _context.Ordini        // Recupera gli ordini dal database includendo i dettagli dell'ordine prodotti e le informazioni sul cliente
                .Include("OrdineDettagli.Orologio") // Include i dettagli dell'ordine e i relativi prodotti (orologi)
                .Include("Cliente")  // Include le informazioni del cliente associato a ogni ordine
                .ToList();
            // Converte ogni ordine in oggetto ListaOrdiniViewModel
            return ordini.Select(ordine =>
            {

                var totaleOrdine = ordine.OrdineDettagli.Sum(d => d.PrezzoUnitario * d.Quantita); // Calcola il totale dell'ordine
                var statoOrdine = ordine.OrdineDettagli.Count > 0 ? "Completato" : "In lavorazione";  // Determina lo stato dell'ordine
                // Recupera l'URL dell'immagine del primo prodotto associato all'ordine se disponibile altrimenti usa un'immagine predefinita
                var urlImmagineProdotto = ordine.OrdineDettagli.Count > 0 && ordine.OrdineDettagli[0].Orologio != null
                    ? ordine.OrdineDettagli[0].Orologio.UrlImmagine
                    : "/img/default.png";

                // Recupera il nome del modello del primo prodotto associato all'ordine se disponibile altrimenti usa "Nessun prodotto"
                var nomeProdotto = ordine.OrdineDettagli.Count > 0 && ordine.OrdineDettagli[0].Orologio != null
                    ? ordine.OrdineDettagli[0].Orologio.Modello
                    : "Nessun prodotto";
                // Crea e restituisce un nuovo oggetto ListaOrdiniViewModel con i dati calcolati e recuperati
                return new ListaOrdiniViewModel
                {
                    Id = ordine.Id,  // ID dell'ordine
                    NomeOrdine = ordine.Nome,  //nome ordine
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

    // Metodo per svuotare il carrello
    public void SvuotaCarrello(string userId, string filePath)
    {
        try
        {
            if (System.IO.File.Exists(filePath)) // Controlla se il file esiste nel path specificato
            {
                var json = System.IO.File.ReadAllText(filePath); // Legge il contenuto del file json
                                                                 // Deserializza il contenuto JSON in un dizionario di carrelli utenti
                                                                 // La chiave è l'ID dell'utente(string), il valore è il carrello dell'utente.
                var carrelliUtenti = JsonConvert.DeserializeObject<Dictionary<string, CarrelloViewModel>>(json) ??
                                     new Dictionary<string, CarrelloViewModel>();

                if (carrelliUtenti.ContainsKey(userId)) // Controlla se esiste un carrello associato all'ID utente(che è chiave del dizionario)
                {
                    // Reimposta il carrello dell'utente a svuotandolo
                    carrelliUtenti[userId] = new CarrelloViewModel
                    {
                        Carrello = new List<OrologioInCarrello>(), // Lista vuota di orologi
                        Totale = 0,  //azzeramento
                        Quantita = 0
                    };

                    var updatedJson = JsonConvert.SerializeObject(carrelliUtenti, Formatting.Indented); // Serializza il carrello aggiornato
                    System.IO.File.WriteAllText(filePath, updatedJson); // Scrive il file aggiornato
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

    //Metodo per caricare il carrello di un utente specifico dal file JSON
    public CarrelloViewModel CaricaCarrello(string userId, string filePath)
    {
        try
        {
            if (System.IO.File.Exists(filePath))
            {
                var json = System.IO.File.ReadAllText(filePath);
                // Deserializza il contenuto JSON in un dizionario
                // La chiave rappresenta l'ID dell'utente e il valore è il relativo carrello
                var carrelliUtenti = JsonConvert.DeserializeObject<Dictionary<string, CarrelloViewModel>>(json) ??
                                     new Dictionary<string, CarrelloViewModel>(); //altrimenti  Usa un dizionario vuoto 
                // Controlla se esiste un carrello per l'utente specifico
                //TryGetValue verifica se un elemento esiste in un dizionario se esiste ottiene il valore associato alla chiave specificata (userId)
                if (carrelliUtenti.TryGetValue(userId, out var carrello))
                {
                    return carrello; //restituisce il carrello associato all'utente
                }
            }
            // Restituisce un carrello vuoto se il file non esiste o il carrello non è trovato
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

    // Azione per creare un ordine dal carrello
    public bool CreaOrdineDaCarrello(string userId, CarrelloViewModel carrello)
    {
        try
        {
            // Recupera il cliente dal database usando l'ID utente
            var cliente = _context.Clienti.FirstOrDefault(c => c.Id == userId);
            if (cliente == null) return false;
            // Crea un nuovo oggetto Ordine
            var nuovoOrdine = new Ordine
            {
                ClienteId = userId, // Assegna l'ID del cliente
                Cliente = cliente,
                DataAcquisto = DateTime.Now,
                Nome = $"Ordine-{DateTime.Now.Ticks}_{userId}" //crea un nome univoco per l'ordine
            };
            // Itera su tutti gli elementi del carrello
            foreach (var item in carrello.Carrello)
            {
                // Recupera il prodotto dal database usando l'ID del prodotto
                var prodotto = _context.Orologi.FirstOrDefault(p => p.Id == item.Orologio.Id);
                if (prodotto == null) continue; // Salta anche se se il prodotto non è stato trovato
                // Aggiunge un nuovo dettaglio ordine al nuovo ordine
                nuovoOrdine.OrdineDettagli.Add(new OrdineDettaglio
                {
                    Ordine = nuovoOrdine, // Associa il dettaglio all'ordine
                    Orologio = prodotto,
                    Quantita = item.QuantitaInCarrello,  // Imposta la quantità
                    PrezzoUnitario = prodotto.Prezzo        //imposta il prezzo unitario
                });
            }
            // Aggiunge il nuovo ordine al  database
            _context.Ordini.Add(nuovoOrdine);
            _context.SaveChanges();

            return true; // Restituisce true se l'ordine è stato creato con successo
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
            // Variabile per conservare l'ordine da eliminare
            Ordine ordineDaEliminare = null;

            // Itera sugli ordini per trovare l'ordine con l'ID specificato
            foreach (var ordine in _context.Ordini.Include("OrdineDettagli.Orologio"))
            {
                if (ordine.Id == id) // Confronta l'ID
                {
                    ordineDaEliminare = ordine; // Assegna l'ordine trovato
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
                if (dettaglio.Orologio != null) // Verifica che l'orologio associato non sia null
                {
                    dettaglio.Orologio.Giacenza += dettaglio.Quantita; // Aggiunge la quantità del prodotto alla giacenza
                    _context.Entry(dettaglio.Orologio).State = EntityState.Modified; // Segnala a Entity Framework che l'orologio è stato modificato
                }
                _context.Entry(dettaglio).State = EntityState.Deleted; // Segnala che il dettaglio dell'ordine deve essere eliminato.
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

    //Metodo per ottenere i dettagli di un ordine specifico
    public DettaglioOrdineViewModel GetDettaglioOrdine(int id)
    {
        try
        {
            // Recupera tutti gli ordini dal database inclusi i dettagli e i clienti
            List<Ordine> ordini = _context.Ordini
                .Include("OrdineDettagli.Orologio") // Include i dettagli dell'ordine e i relativi oggetti Orologio
                .Include("Cliente") // Include le informazioni sul cliente
                .ToList(); //converte in lista

            // Trova l'ordine con l'ID specificato
            Ordine ordine = null;       // Inizializza la variabile ordine a null
            foreach (var o in ordini)   //itera su tutti gli ordini recuperati
            {
                if (o.Id == id)     // Controlla se l'ID dell'ordine corrente corrisponde a quello cercato
                {
                    ordine = o;  // Assegna l'ordine trovato
                    break; // Esce dal ciclo una volta trovato l'ordine
                }
            }

            // Controlla se l'ordine esiste
            if (ordine == null)
            {
                _logger.LogWarning("Ordine con ID {Id} non trovato.", id);
                return null;
            }

            // Crea un oggetto ViewModel per rappresentare i dettagli dell'ordine
            DettaglioOrdineViewModel viewModel = new DettaglioOrdineViewModel
            {
                OrdineId = ordine.Id, // ID dell'ordine
                NomeOrdine = ordine.Nome,
                ClienteNome = ordine.Cliente != null ? ordine.Cliente.Nome : "Cliente sconosciuto", // Nome del cliente o un valore di default
                IndirizzoSpedizione = "Via Esempio, 123", // Valore statico
                MetodoPagamento = "Carta di credito",     // Valore statico
                TipoSpedizione = "Standard",             // Valore statico
                StatoOrdine = ordine.OrdineDettagli.Count > 0 ? "Completato" : "In lavorazione",
                DataAcquisto = ordine.DataAcquisto,
                Subtotale = 0m, // Inizializza il subtotale a zero
                CostoSpedizione = 10.00m, // Valore fisso
                Totale = 0m,
                Prodotti = new List<DettaglioOrdineProdottoViewModel>() // Inizializza una lista vuota per i prodotti
            };

            // Calcola il totale e aggiungi i dettagli del prodotto al ViewModel
            foreach (var dettaglio in ordine.OrdineDettagli) // Itera sui dettagli dell'ordine
            {
                var prezzoTotaleDettaglio = dettaglio.PrezzoUnitario * dettaglio.Quantita; // Calcola il prezzo totale
                viewModel.Subtotale += prezzoTotaleDettaglio;

                // Crea un ViewModel per il prodotto associato al dettaglio
                DettaglioOrdineProdottoViewModel prodottoViewModel = new DettaglioOrdineProdottoViewModel
                {
                    UrlImmagine = dettaglio.Orologio != null ? dettaglio.Orologio.UrlImmagine : "/img/default.png", // URL immagine del prodotto o valore predefinito
                    Modello = dettaglio.Orologio != null ? dettaglio.Orologio.Modello : "Modello sconosciuto",
                    Quantita = dettaglio.Quantita,   // Quantità acquistata
                    PrezzoUnitario = dettaglio.PrezzoUnitario,
                    Descrizione = $"Quantità: {dettaglio.Quantita} - Prezzo unitario: €{dettaglio.PrezzoUnitario}",
                    Giacenza = dettaglio.Orologio != null ? dettaglio.Orologio.Giacenza : 0
                };

                viewModel.Prodotti.Add(prodottoViewModel); // Aggiunge il prodotto al ViewModel
            }

            // Calcola il totale dell'ordine quindi prezzo prodotti + spese spedizione
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