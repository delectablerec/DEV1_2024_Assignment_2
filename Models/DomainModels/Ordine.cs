public class Ordine : General
{
    // Nome ordine
    public override string Nome { get { return $"BRT-{Id}_{Cliente!.Id}"; } }

    // Data in cui è stato effettuato l'acquisto
    public DateTime DataAcquisto { get; set; }

    // Quantità del prodotto acquistato
    public int Quantita { get; set; }

    // Cliente associato all'ordine
    public string ClienteId { get; set; }
    public Cliente Cliente { get; set; }

    
 //   public int OrologioId { get; set; }
  //  public Orologio? Orologio { get; set; }  

       // Stato dell'ordine
      public StatoOrdine StatoOrdine { get; set; } = StatoOrdine.InLavorazione;

      // Lista di orologi associati all'ordine
    public List<Orologio> Orologi { get; set; } = new List<Orologio>();
}

// Definizione dell'enum StatoOrdine
public enum StatoOrdine
{
    InLavorazione,
    Completato,
    Rimosso
}