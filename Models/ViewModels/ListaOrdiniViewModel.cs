//raccoglie solo i dati rilevanti per una vista che mostra una lista di ordini
public class ListaOrdiniViewModel
{
    public int Id { get; set; }
    public string NomeOrdine { get; set; } = string.Empty;
    public DateTime DataAcquisto { get; set; }
    public string StatoOrdine { get; set; } = "In lavorazione";
    public decimal TotaleOrdine { get; set; }
    public string? UrlImmagineProdotto { get; set; }
    public string NomeProdotto { get; set; } = "Nessun prodotto";
    public decimal CostoSpedizione { get; set; } = 10.00m; // Default
}

