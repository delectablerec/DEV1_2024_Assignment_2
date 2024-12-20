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


// Metodo per svuotare il carrello
  public void SvuotaCarrello(string userId, string filePath)
{
    try
    {
        if (File.Exists(filePath)) // Controlla se il file esiste nel path specificato
        {
            var json = File.ReadAllText(filePath); // Legge il contenuto del file json
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
                File.WriteAllText(filePath, updatedJson); // Scrive il file aggiornato
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


public List<ListaOrdiniViewModel> GetOrdini()
{
    try
    {
        // Recupera gli ordini dal database includendo i dettagli dell'ordine e il cliente
        var ordini = _context.Ordini
            .Include("OrdineDettagli.Orologio")
            .Include("Cliente")
            .ToList();

        // Crea una lista per memorizzare i risultati
        var listaOrdiniViewModel = new List<ListaOrdiniViewModel>();

        // Itera su ciascun ordine
        foreach (var ordine in ordini)
        {
            // Calcola il totale dell'ordine
            decimal totaleOrdine = 0;
            foreach (var dettaglio in ordine.OrdineDettagli)
            {
                totaleOrdine += dettaglio.PrezzoUnitario * dettaglio.Quantita;
            }

            // Determina lo stato dell'ordine
            string statoOrdine = ordine.OrdineDettagli.Count > 0 ? "Completato" : "In lavorazione";

            // Recupera l'URL dell'immagine del primo prodotto se disponibile, altrimenti usa un'immagine predefinita
            string urlImmagineProdotto = "/img/default.png";
            if (ordine.OrdineDettagli.Count > 0 && ordine.OrdineDettagli[0].Orologio != null)
            {
                urlImmagineProdotto = ordine.OrdineDettagli[0].Orologio.UrlImmagine;
            }

            // Recupera il nome del modello del primo prodotto se disponibile, altrimenti usa "Nessun prodotto"
            string nomeProdotto = "Nessun prodotto";
            if (ordine.OrdineDettagli.Count > 0 && ordine.OrdineDettagli[0].Orologio != null)
            {
                nomeProdotto = ordine.OrdineDettagli[0].Orologio.Modello;
            }

            List<Orologio> orologi = new List<Orologio>();
            foreach (OrdineDettaglio ordineDettaglio in ordine.OrdineDettagli)
            {
                orologi.Add(ordineDettaglio.Orologio);
            }
            // Crea un oggetto ListaOrdiniViewModel per l'ordine corrente
            var viewModel = new ListaOrdiniViewModel
            {
                Id = ordine.Id, // ID dell'ordine
                NomeOrdine = ordine.Nome, // Nome dell'ordine
                DataAcquisto = ordine.DataAcquisto, // Data dell'acquisto
                StatoOrdine = statoOrdine, // Stato calcolato
                TotaleOrdine = totaleOrdine, // Totale calcolato
                Orologi = orologi, // URL immagine prodotto
                NomeProdotto = nomeProdotto, // Nome del prodotto
                CostoSpedizione = 10.00m // Costo spedizione fisso
            };

            // Aggiungi il ViewModel alla lista
            listaOrdiniViewModel.Add(viewModel);
        }

        // Restituisce la lista degli ordini
        return listaOrdiniViewModel;
    }
    catch (Exception ex)
    {
        // Logga l'errore e rilancia l'eccezione
        _logger.LogError($"Errore durante il recupero degli ordini: {ex.Message}");
        throw;
    }
}


    //Metodo per caricare il carrello di un utente specifico dal file JSON
    public CarrelloViewModel CaricaCarrello(string userId, string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
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



// Metodo per creare un ordine dal carrello
    public bool CreaOrdineDaCarrello(string userId, CarrelloViewModel carrello)
{
    try
    {
        // Recupera il cliente dal database usando l'ID utente
        Cliente cliente = null; // Inizializza una variabile per memorizzare il cliente trovato

        foreach (var c in _context.Clienti) // Itera su tutti i clienti nel database
        {
            if (c.Id == userId)  // Confronta l'ID di ciascun cliente con l'ID dell'utente specificato
            {
                cliente = c; // Se c'è una corrispondenza, assegna il cliente alla variabile
                break; // Termina il ciclo una volta trovato il cliente
            }
        }

        if (cliente == null) return false; // Restituisce false se il cliente non è trovato quindi l'ordine non verrà creato

        // Crea un nuovo oggetto Ordine  per memorizzare i dettagli dell'ordine
        var nuovoOrdine = new Ordine
        {
            ClienteId = userId, // Assegna l'ID del cliente
            Cliente = cliente, // Associa l'oggetto cliente recuperato
            DataAcquisto = DateTime.Now,
            Nome = $"Ordine-{DateTime.Now.Ticks}_{userId}" // Crea un nome univoco per l'ordine  usando un timestamp e l'ID utente
        };

        // Itera su tutti gli elementi del carrello
        foreach (var item in carrello.Carrello)
        {
            // Recupera il prodotto dal database usando l'ID del prodotto
            Orologio prodotto = null; // Inizializza una variabile per memorizzare il prodotto trovato

            foreach (var p in _context.Orologi) // Itera su tutti gli orologi nel database
            {
                if (p.Id == item.Orologio.Id) //  Confronta l'ID dell'orologio con quello del prodotto nel carrello
                {
                    prodotto = p; // Se c'è una corrispondenza, assegna il prodotto alla variabile
                    break; // Termina il ciclo una volta trovato il prodotto
                }
            }

            if (prodotto == null) continue; // Salta l'elemento corrente del carrello se il prodotto non è stato trovato

             // Crea un nuovo dettaglio dell'ordine per memorizzare i dettagli del prodotto
            nuovoOrdine.OrdineDettagli.Add(new OrdineDettaglio
            {
                Ordine = nuovoOrdine, // Associa il dettaglio all'ordine
                Orologio = prodotto, // Associa il prodotto trovato
                Quantita = item.QuantitaInCarrello, // Imposta la quantità
                PrezzoUnitario = prodotto.Prezzo   // Imposta il prezzo unitario
            });
        }

        // Aggiunge il nuovo ordine al database
        _context.Ordini.Add(nuovoOrdine);
        _context.SaveChanges(); // Salva le modifiche nel database

        return true; // Restituisce true se l'ordine è stato creato con successo
    }
    catch (Exception ex)
    {
        _logger.LogError($"Errore durante la creazione dell'ordine: {ex.Message}");
        return false; // Restituisce false in caso di errore
    }
}

//Metodo per eliminare l'ordine
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
              
                }
              
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


