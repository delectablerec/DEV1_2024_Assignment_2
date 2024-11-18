public class DettaglioOrdineProdottoViewModel
{
    public string UrlImmagine { get; set; } // URL immagine del prodotto
    public string Modello { get; set; } // Modello del prodotto
    public int Quantita { get; set; } // Quantità ordinata
    public decimal PrezzoUnitario { get; set; } // Prezzo unitario
    public string Descrizione { get; set; } // Descrizione aggiuntiva
    public int Giacenza { get; set; } // Aggiungi la proprietà Giacenza se necessaria
}
