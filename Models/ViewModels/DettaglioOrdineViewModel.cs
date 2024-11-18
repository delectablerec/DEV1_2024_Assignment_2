
public class DettaglioOrdineViewModel
{
    public int OrdineId { get; set; } // ID dell'ordine
    public string NomeOrdine { get; set; } // Nome o numero identificativo dell'ordine
    public string ClienteNome { get; set; } // Nome del cliente
    //public string ClienteTelefono { get; set; } // Telefono del cliente
    public string IndirizzoSpedizione { get; set; } // Indirizzo di spedizione
    public string MetodoPagamento { get; set; } // Metodo di pagamento
    public string TipoSpedizione { get; set; } // Tipo di spedizione
    public decimal CostoSpedizione { get; set; } // Costo di spedizione
    public string StatoOrdine { get; set; } // Stato dell'ordine (enum convertito in stringa)
    public DateTime DataAcquisto { get; set; } // Data di acquisto
    public List<Orologio> Prodotti { get; set; } // Lista dei prodotti nell'ordine
     public CarrelloViewModel Carrello { get; set; } = new CarrelloViewModel();

     public decimal Subtotale { get; set; }
     public decimal Totale {get;set;}
}

