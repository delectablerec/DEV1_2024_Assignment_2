using Newtonsoft.Json;

public class CarrelloService
{
    //private Dictionary<string, CarrelloViewModel> _userCart = new();     // Carrello collegato al cliente
    private readonly ApplicationDbContext _context;
    private const string CartFilePath = "wwwroot/json/carrelli.json";
    private readonly ILogger<CarrelloService> _logger;

    public CarrelloService(ApplicationDbContext context, ILogger<CarrelloService> logger)
    {
        _context = context;
        _logger = logger;
    }
    public CarrelloViewModel CaricaCarrello(string userId)
    {
        try
        {
            Dictionary<string, CarrelloViewModel> carrelliUtenti = new();

            // Controlla se il file json esiste e lo popola
            if (File.Exists(CartFilePath))
            {
                var json = File.ReadAllText(CartFilePath);
                carrelliUtenti = JsonConvert.DeserializeObject<Dictionary<string, CarrelloViewModel>>(json)
                        ?? new Dictionary<string, CarrelloViewModel>();
            }

            // Controlla se il carrello utente esiste
            if (carrelliUtenti.TryGetValue(userId, out var existingCart))
            {
                _logger.LogInformation("Cart loaded for UserId: {UserId}", userId);
                return existingCart; // Ritorna il carrello dell'utente
            }

            // Inizializza un nuovo carrello se l'utente non ne ha già uno
            _logger.LogInformation("No cart found for UserId: {UserId}. Initializing a new one.", userId);
            var newCart = new CarrelloViewModel
            {
                Carrello = new List<OrologioInCarrello>(),
                Totale = 0,
                Quantita = 0
            };

            // Aggiunge il carrello alla collezione
            carrelliUtenti[userId] = newCart;
            SalvaTuttiICarrelli(carrelliUtenti);

            return newCart;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error loading cart for UserId: {UserId}. Exception: {Message}", userId, ex.Message);
            throw;
        }
    }
    public bool AggiungiACarrello(string userId, Orologio orologio)
    {
        try
        {
            // Carica o inizializza un carrello
            var carrello = CaricaCarrello(userId);

            // Controlla che il prodotto sia in stock
            int stock = 0;
            foreach (var item in _context.Orologi)
            {
                if (item.Id == orologio.Id)
                {
                    stock = item.Giacenza;
                    break;
                }
            }

            if (stock <= 0)
            {
                _logger.LogWarning("Prodotto con ID: {IdProdotto} non è disponibile", orologio.Id);
                return false; //Ritorno anticipato se il prodotto non è in stock
            }

            // Cerca il prodotto nel carrello e lo assegna ad una nuova variabile
            OrologioInCarrello prodottoInCarrello = null;
            foreach (var item in carrello.Carrello)
            {
                if (item.OrologioId == orologio.Id)
                {
                    prodottoInCarrello = item;
                    break;
                }
            }

            // Se il prodotto è già presente nel carrello
            if (prodottoInCarrello != null)
            {
                prodottoInCarrello.QuantitaInCarrello++; // Aumenta quantità prodotto
                _logger.LogInformation("Prodotto aggiunto al carrello. Prodotto ID: {IdProdotto} Quantità: {QuantitaProdotto}", orologio.Id, prodottoInCarrello.QuantitaInCarrello);
            }
            else // Se il prodotto non c'è ancora
            {
                carrello.Carrello.Add(new OrologioInCarrello    // Crea una nuova istanza di orologio in carrello e la aggiunge
                {
                    OrologioId = orologio.Id,
                    Orologio = orologio,    // Setta la corrispondenza delle proprietà di orologio
                    QuantitaInCarrello = 1  // Setta la quantità per la prima volta (a 1)
                });
                _logger.LogInformation("Prodotto aggiunto a carrello per la prima volta, Prodotto ID: {IdProdotto}", orologio.Id);
            }

            // Aggiorna totale e quantità
            decimal totale = 0;
            int quantita = 0;
            foreach (var item in carrello.Carrello)
            {
                totale += item.Orologio.Prezzo * item.QuantitaInCarrello;   // Tiene conto della quantità del prodotto
                quantita += item.QuantitaInCarrello;
            }
            carrello.Totale = totale;
            carrello.Quantita = quantita;

            _logger.LogInformation("Carrello aggiornato per UserID: {UserId}, Totale: {Totale}, Quantità Prodotti: {QuantitaProdotti}", userId, totale, quantita);

            // Diminuisce lo stock nel database del prodotto aggiunto al carrello
            orologio.Giacenza--;
            _context.SaveChanges();

            var carrelliUtenti = new Dictionary<string, CarrelloViewModel>();
            carrelliUtenti[userId] = carrello;
            // Salva tutti i carrelli con il nuovo prodotto nel carrello giusto
            SalvaTuttiICarrelli(carrelliUtenti);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError("Errore durante l'aggiunta al carrello UserId: {UserId}, Id Prodotto: {IdProdotto}, Exception: {Message}", userId, orologio.Id, ex.Message);
            return false; // Failure in case of error
        }
    }
    public bool RimuoviUnoDalCarrello(string userId, Orologio orologio)
    {
        try
        {
            // Carica o inizializza un carrello
            var carrello = CaricaCarrello(userId);

            // Cerca il prodotto nel carrello e lo assegna ad una nuova variabile
            OrologioInCarrello prodottoInCarrello = null;
            foreach (var item in carrello.Carrello)
            {
                if (item.OrologioId == orologio.Id)
                {
                    prodottoInCarrello = item;
                    break;
                }
            }

            // Se il prodotto è presente nel carrello
            if (prodottoInCarrello != null)
            {
                if (prodottoInCarrello.QuantitaInCarrello <= 1)
                    RimuoviDalCarrello(userId, prodottoInCarrello.OrologioId);
                else
                    {
                        prodottoInCarrello.QuantitaInCarrello--; // Riduci quantità prodotto
                    _logger.LogInformation("Prodotto aggiunto al carrello. Prodotto ID: {IdProdotto} Quantità: {QuantitaProdotto}", orologio.Id, prodottoInCarrello.QuantitaInCarrello);
                    }
            }

            // Aggiorna totale e quantità
            decimal totale = 0;
            int quantita = 0;
            foreach (var item in carrello.Carrello)
            {
                totale += item.Orologio.Prezzo * item.QuantitaInCarrello;   // Tiene conto della quantità del prodotto
                quantita += item.QuantitaInCarrello;
            }
            carrello.Totale = totale;
            carrello.Quantita = quantita;

            _logger.LogInformation("Carrello aggiornato per UserID: {UserId}, Totale: {Totale}, Quantità Prodotti: {QuantitaProdotti}", userId, totale, quantita);

            // Aumenta lo stock nel database del prodotto aggiunto al carrello
            orologio.Giacenza++;
            _context.SaveChanges();

            var carrelliUtenti = new Dictionary<string, CarrelloViewModel>();
            carrelliUtenti[userId] = carrello;
            // Salva tutti i carrelli con il nuovo prodotto nel carrello giusto
            SalvaTuttiICarrelli(carrelliUtenti);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError("Errore durante la rimozione dal carrello UserId: {UserId}, Id Prodotto: {IdProdotto}, Exception: {Message}", userId, orologio.Id, ex.Message);
            return false; // Failure in case of error
        }
    }

    public bool RimuoviDalCarrello(string userId, int orologioId)
    {
        try
        {
            // Carica o inizializza il carrello per l'utente
            var carrello = CaricaCarrello(userId);

            // Trova il prodotto nel carrello
            OrologioInCarrello prodottoInCarrello = null;
            foreach (var item in carrello.Carrello)
            {
                if (item.OrologioId == orologioId)
                {
                    prodottoInCarrello = item;
                    break;
                }
            }

            if (prodottoInCarrello == null)
            {
                _logger.LogWarning("Prodotto con ID: {IdProdotto} non trovato nel carrello per l'utente: {UserId}", orologioId, userId);
                return false; // Se il prodotto non è nel carrello, ritorna falso
            }

            // Aggiusta lo stock nel database per il prodotto rimosso
            var orologio = CercaProdottoPerId(_context.Orologi.ToList(), orologioId);
            if (orologio != null)
            {
                orologio.Giacenza += prodottoInCarrello.QuantitaInCarrello; // Ripristina lo stock
                _context.SaveChanges(); // Salva i cambiamenti nel database
                _logger.LogInformation("Stock aggiornato per il prodotto ID: {IdProdotto}, Nuovo stock: {Stock}", orologioId, orologio.Giacenza);
            }

            // Rimuovi il prodotto dal carrello
            carrello.Carrello.Remove(prodottoInCarrello);

            // Ricalcola il totale e la quantità del carrello
            decimal totale = 0;
            int quantita = 0;
            foreach (var item in carrello.Carrello)
            {
                totale += item.Orologio.Prezzo * item.QuantitaInCarrello; // Calcola il totale considerando la quantità
                quantita += item.QuantitaInCarrello; // Calcola la quantità totale
            }
            carrello.Totale = totale;
            carrello.Quantita = quantita;

            _logger.LogInformation("Carrello aggiornato per l'utente {UserId}, Nuovo totale: {Totale}, Nuova quantità: {Quantita}", userId, totale, quantita);

            // Salva i carrelli aggiornati nel file JSON
            Dictionary<string, CarrelloViewModel> carrelliUtenti = new();
            carrelliUtenti[userId] = carrello;
            SalvaTuttiICarrelli(carrelliUtenti);

            return true; // Success
        }
        catch (Exception ex)
        {
            _logger.LogError("Errore durante la rimozione dal carrello per l'utente {UserId}, Id Prodotto: {IdProdotto}, Exception: {Message}", userId, orologioId, ex.Message);
            return false; // Failure in case of error
        }
    }

    private void SalvaTuttiICarrelli(Dictionary<string, CarrelloViewModel> carrelliUtenti)
    {
        try
        {
            var json = JsonConvert.SerializeObject(carrelliUtenti, Formatting.Indented);
            File.WriteAllText(CartFilePath, json); // Save the updated carts to the JSON file
            _logger.LogInformation("Carrello salvato su {FilePath}", CartFilePath);
        }
        catch (Exception ex)
        {
            _logger.LogError("Errore durante il salvataggio sul file: {Message}", ex.Message);
        }
    }

    public Orologio CercaProdottoPerId(List<Orologio> orologi, int id)
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
            _logger.LogError("Errore nella ricerca : {Message} \n Exception Type : {ExceptionType} \n Stack Trace : {StackTrace}", ex.Message, ex.GetType().Name, ex.StackTrace);
            return null;
        }
    }
}