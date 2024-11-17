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

/*  NON PIU NECESSARIO
    public void InizializzaCarrello(string userId)
    {
        if (!_userCart.ContainsKey(userId))
        {
            _userCart[userId] = new CarrelloViewModel
            {
                Carrello = new List<OrologioInCarrello>(),
                Totale = 0,
                Quantita = 0
            };
            Console.WriteLine($"Initialized cart for UserId: {userId}");
        }
        else
        {
            Console.WriteLine($"Cart already initialized for UserId: {userId}");
        }
        SalvaCarrelloSuJson();
    }
*/

/*
    public CarrelloViewModel CaricaCarrello(string userId)
    {
        _logger.LogInformation("Loading cart for UserId: {UserId}", userId);
        if (_userCart.TryGetValue(userId, out var cart))
        {
            return cart;
        }

        _logger.LogWarning("Cart not found for UserId: {UserId}. Returning an empty cart.", userId);

        // Return a new empty cart if it does not exist
        return new CarrelloViewModel
        {
            Carrello = new List<OrologioInCarrello>(),
            Totale = 0,
            Quantita = 0
        };
    }
*/
    public CarrelloViewModel CaricaCarrello(string userId)
    {
        try
        {
            Dictionary<string, CarrelloViewModel> carrelliUtenti = new();

            // Check if the JSON file exists and load it
            if (File.Exists(CartFilePath))
            {
                var json = File.ReadAllText(CartFilePath);
                carrelliUtenti = JsonConvert.DeserializeObject<Dictionary<string, CarrelloViewModel>>(json) 
                        ?? new Dictionary<string, CarrelloViewModel>();
            }

            // Check if the user's cart exists
            if (carrelliUtenti.TryGetValue(userId, out var existingCart))
            {
                _logger.LogInformation("Cart loaded for UserId: {UserId}", userId);
                return existingCart; // Return the user's cart
            }

            // Initialize a new cart if the user does not have one
            _logger.LogInformation("No cart found for UserId: {UserId}. Initializing a new one.", userId);
            var newCart = new CarrelloViewModel
            {
                Carrello = new List<OrologioInCarrello>(),
                Totale = 0,
                Quantita = 0
            };

            // Add the new cart to the collection and save it
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
            foreach(var item in _context.Orologi)
            {
                if(item.Id == orologio.Id)
                {
                    stock = item.Giacenza;
                    break;
                }
            }

            if(stock <= 0)
            {
                _logger.LogWarning("Prodotto con ID: {IdProdotto} non è disponibile", orologio.Id);
                return false; //Ritorno anticipato se il prodotto non è in stock
            }

            // Cerca il prodotto nel carrello e lo assegna ad una nuova variabile
            OrologioInCarrello prodottoInCarrello = null;
            foreach(var item in carrello.Carrello)
            {
                if(item.OrologioId == orologio.Id)
                {
                    prodottoInCarrello = item;
                    break;
                }
            }

            // Se il prodotto è già presente nel carrello
            if(prodottoInCarrello != null)
            {
                prodottoInCarrello.QuantitaInCarrello ++; // Aumenta quantità prodotto
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
            foreach(var item in carrello.Carrello)
            {
                totale += item.Orologio.Prezzo * item.QuantitaInCarrello;   // Tiene conto della quantità del prodotto
                quantita += item.QuantitaInCarrello;
            }
            carrello.Totale = totale;
            carrello.Quantita = quantita;

            _logger.LogInformation("Carrello aggiornato per UserID: {UserId}, Totale: {Totale}, Quantità Prodotti: {QuantitaProdotti}", userId, totale, quantita);

            // Diminuisce lo stock nel database del prodotto aggiunto al carrello
            orologio.Giacenza --;
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


/*    public void AggiungiACarrello(string userId, Orologio orologio)
    {
        if (!_userCart.ContainsKey(userId))
        {
            InizializzaCarrello(userId);
        }

        var carrello = _userCart[userId];

        // Check if the product is already in the cart
        var prodottoInCarrello = carrello.Carrello.FirstOrDefault(item => item.OrologioId == orologio.Id);
        if (prodottoInCarrello != null)
        {
            prodottoInCarrello.QuantitaInCarrello++;
            _logger.LogInformation("Product added to cart. ProductId: {ProductId}, Quantity: {Quantity}", orologio.Id, prodottoInCarrello.QuantitaInCarrello);
        }
        else
        {
            carrello.Carrello.Add(new OrologioInCarrello
            {
                OrologioId = orologio.Id,
                Orologio = orologio,
                QuantitaInCarrello = 1
            });
            _logger.LogInformation("Product added to cart for the first time. ProductId: {ProductId}", orologio.Id);
        }

        // Update the total and quantity
        decimal totale = 0;
        int quantita = 0;
        foreach (var item in carrello.Carrello)
        {
            totale += item.Orologio.Prezzo * item.QuantitaInCarrello;
            quantita += item.QuantitaInCarrello;
        }

        carrello.Totale = totale;
        carrello.Quantita = quantita;
        _logger.LogInformation("Cart updated for UserId: {UserId}, Total: {Total}, Quantity: {Quantity}", userId, totale, quantita);
        SalvaCarrelloSuJson();
    }

    // Salva il carrello su un file json
    private void SalvaCarrelloSuJson()
    {
        try
        {
            var json = JsonConvert.SerializeObject(_userCart);
            File.WriteAllText(CartFilePath, json); // Save all carts to the JSON file
            _logger.LogInformation("Carts saved to {FilePath}", CartFilePath);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error saving carts to file: {Message}", ex.Message);
        }
    }
*/
}
