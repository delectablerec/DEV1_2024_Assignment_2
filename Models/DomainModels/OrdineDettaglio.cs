//classe rappresenta i dettagli di un ordine specifico cnsente di gestire relazioni uno-a-molti tra ordini e prodotti
public class OrdineDettaglio
{
    public int Id { get; set; }

    

      // Chiave esterna che collega il dettaglio all'ordine
    public int OrdineId { get; set; }

    // Relazione con Ordine
    public Ordine Ordine { get; set; } = null!;

    // Chiave esterna che collega il dettaglio al prodotto (orologio)
    public int OrologioId { get; set; }
    public Orologio Orologio { get; set; } = null!;

    // Quantità acquistata di questo prodotto
    public int Quantita { get; set; }

    // Prezzo unitario al momento dell'ordine
    public decimal PrezzoUnitario { get; set; }
}
