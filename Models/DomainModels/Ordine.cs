/*public class Ordine : General


{
    // Nome ordine
    //public override string? Nome { get { return $"BRT-{Id}_{Cliente!.Id}"; } }
public override string Nome { get; set; } = "Default-Order-Name";
    // Data in cui è stato effettuato l'acquisto
    public DateTime DataAcquisto { get; set; }

    // Quantità del prodotto acquistato
    public int Quantita { get; set; }

    // Cliente associato all'ordine
    public string ClienteId { get; set; }
    public Cliente Cliente { get; set; }

    // Metodo di pagamento
    public string MetodoPagamento { get; set; }

      public decimal CostoSpedizione { get; set; }

    
 //   public int OrologioId { get; set; }
  //  public Orologio? Orologio { get; set; }  

       // Stato dell'ordine
      public StatoOrdine StatoOrdine { get; set; } = StatoOrdine.InLavorazione;

      // Lista di orologi associati all'ordine
    public List<Orologio> Orologi { get; set; } = new List<Orologio>();
    public string IndirizzoSpedizione { get; set; }
}

// Definizione dell'enum StatoOrdine
public enum StatoOrdine
{
    InLavorazione,
    Completato,
    Rimosso
}*/

public class Ordine : General
{
    // Nome ordine with a default value
    public override string Nome { get; set; } = "Default-Order-Name";

    // Data in cui è stato effettuato l'acquisto
    public DateTime DataAcquisto { get; set; } = DateTime.Now;

    // Quantità del prodotto acquistato
    public int Quantita { get; set; }

    // Cliente associato all'ordine
    public string ClienteId { get; set; } = string.Empty; // Ensure it's never null
    public Cliente Cliente { get; set; } = null!; // Marked as non-nullable but must be assigned at runtime

    // Metodo di pagamento with a default value
   // public string MetodoPagamento { get; set; } = "credit cart";

    // Costo di spedizione
  //  public decimal CostoSpedizione { get; set; }

    // Stato dell'ordine with a default value
  //  public StatoOrdine StatoOrdine { get; set; } = StatoOrdine.InLavorazione;

    // Lista di orologi associati all'ordine
    public List<Orologio> Orologi { get; set; } = new List<Orologio>();

    // Indirizzo di spedizione
  //  public string? IndirizzoSpedizione { get; set; } // Ensure it's never null
}

// Definizione dell'enum StatoOrdine
/*public enum StatoOrdine
{
    InLavorazione,
    Completato,
    Rimosso
}*/
