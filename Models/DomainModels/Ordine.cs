
public class Ordine : General
{
    

    public override string Nome
    {
        get
        {
            if (Id == 0 || Cliente == null)
            {
                return "Ordine-0000"; // in caso di dati incompleti
            }
            return $"BRT-{Id}_{Cliente.Id}";
        }
    }

    // Data in cui è stato effettuato l'acquisto
    public DateTime DataAcquisto { get; set; } = DateTime.Now;

    // Quantità del prodotto acquistato
    public int Quantita { get; set; }

    // Cliente associato all'ordine
    public string ClienteId { get; set; } = string.Empty; // non deve essere nullo
    public Cliente Cliente { get; set; } = null!; // marcato non-nullable

    public List<Orologio> Orologi { get; set; } = new List<Orologio>();

}



    // Metodo di pagamento with a default value
   // public string MetodoPagamento { get; set; } = "credit cart";

    // Costo di spedizione
  //  public decimal CostoSpedizione { get; set; }

    // Stato dell'ordine with a default value
  //  public StatoOrdine StatoOrdine { get; set; } = StatoOrdine.InLavorazione;

    // Lista di orologi associati all'ordine
    

    // Indirizzo di spedizione
  //  public string? IndirizzoSpedizione { get; set; } // Ensure it's never null


// Definizione dell'enum StatoOrdine
/*public enum StatoOrdine
{
    InLavorazione,
    Completato,
    Rimosso
}*/
