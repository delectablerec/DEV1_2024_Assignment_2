public class ProfiloUtenteViewModel
{
    public Cliente Cliente { get; set; }
    public List<Ordine> Ordini { get; set; }
    public List<Orologio> Wishlist { get; set; }
    public string NomeUtente { get; set; } // Per l'integrazione con Identity
    public string Email { get; set; } // Per l'integrazione con Identity
}
