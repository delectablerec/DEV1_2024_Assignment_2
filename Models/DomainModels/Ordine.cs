public class Ordine : General
{
    // Nome of the order
    public override string Nome { get { return $"BRT-{Id}_{Cliente!.Id}"; } }

    // Data in cui è stato effettuato l'acquisto
    public DateTime DataAcquisto { get; set; }

    // Quantità del prodotto acquistato
    public int Quantita { get; set; }

    // Cliente associato all'ordine
    public Cliente Cliente { get; set; }

    // Orologio associato all'ordine (formerly Prodotto)
    public Orologio? Orologio { get; set; }  // Change this to Orologio
}