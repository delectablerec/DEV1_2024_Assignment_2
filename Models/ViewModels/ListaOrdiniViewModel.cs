/*public class ListaOrdiniViewModel
{
    public int Id { get; set; } // ID dell'ordine
    public string NomeOrdine { get; set; } // Nome o numero identificativo dell'ordine
    public DateTime DataAcquisto { get; set; } // Data di acquisto dell'ordine
    public string StatoOrdine { get; set; } // Stato dell'ordine
    public decimal TotaleOrdine { get; set; } // Totale dell'ordine
    public string UrlImmagineProdotto { get; set; } // URL immagine del primo prodotto
    public string NomeProdotto { get; set; } // Nome del primo prodotto nell'ordine
    public decimal CostoSpedizione { get; set; } // Costo di spedizione
}*/

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
    public List<Orologio> Orologi { get; set; }
}

