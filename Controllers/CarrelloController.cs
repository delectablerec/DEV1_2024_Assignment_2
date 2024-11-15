using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

public class CarrelloController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<Cliente> _userManager;

    public CarrelloController(ApplicationDbContext context, UserManager<Cliente> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> Acquista()
    {
        // Recupera il carrello dalla sessione o dal database
        var carrello = OttieniCarrelloUtente();

        if (carrello.Prodotti.Count == 0)
        {
            return RedirectToAction("Index", "Carrello"); // Ritorna al carrello se non ci sono prodotti
        }

        // Recupera il cliente attualmente autenticato
        var cliente = await _userManager.GetUserAsync(User);
        if (cliente == null)
        {
            return RedirectToAction("Login", "Account"); // Se l'utente non è autenticato, reindirizza al login
        }

        // Crea un nuovo ordine
        var ordine = new Ordine
        {
           ClienteId = cliente.Id,
            Cliente = cliente,
            DataAcquisto = DateTime.Now,
            Quantita = carrello.Quantita,
            StatoOrdine = StatoOrdine.InLavorazione,
            Orologi = new List<Orologio>(carrello.Prodotti) // Aggiungi i prodotti del carrello all'ordine
        };

        // Aggiungi l'ordine al contesto e salva
        _context.Ordini.Add(ordine);
        await _context.SaveChangesAsync();

        // Creazione del ViewModel per passare i dettagli dell'ordine e il costo della spedizione alla vista
        var ordineViewModel = new OrdiniViewModel
        {
            Ordine = ordine,
            Cliente = cliente,
            Carrello = carrello,
            DataAcquisto = ordine.DataAcquisto,
            CostoSpedizione = 10m, // Imposta il costo di spedizione a 10
            TipoSpedizione = "Standard", // Tipo di spedizione predefinito
            MetodoPagamento = "Carta di Credito" // Metodo di pagamento di esempio
        };

        // Svuota il carrello dopo l'acquisto
        carrello.Prodotti.Clear();
        carrello.Totale = 0;
        carrello.Quantita = 0;

        // Passa il ViewModel alla vista di conferma
        return View("ConfermaOrdine", ordineViewModel);
    }

    private Carrello OttieniCarrelloUtente()
    {
        // Metodo per recuperare il carrello dell'utente
        return new Carrello
        {
            Prodotti = new List<Orologio>(), // Simula l'ottenimento dei prodotti
            Totale = 2000, // Esempio di totale
            Quantita = 2 // Esempio di quantità
        };
    }
}
