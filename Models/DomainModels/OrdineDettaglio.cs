public class OrdineDettaglio
{
    public int Id { get; set; }

    // Relazione con Ordine
    public int OrdineId { get; set; }
    public Ordine Ordine { get; set; } = null!;

    // Relazione con Orologio
    public int OrologioId { get; set; }
    public Orologio Orologio { get; set; } = null!;

    // Quantità acquistata di questo prodotto
    public int Quantita { get; set; }

    // Prezzo unitario al momento dell'ordine
    public decimal PrezzoUnitario { get; set; }
}
