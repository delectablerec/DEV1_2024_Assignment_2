public class ProfiloAdminViewModel
{
    public List<Cliente> Clienti { get; set; }
    public List<Ordine> Ordini { get; set; }
    public List<Prodotto> Prodotti { get; set; }
    public string NomeUtente { get; set; } // Per l'integrazione con Identity
}
