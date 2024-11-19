public class DettaglioOrdineViewModel
{
    public int OrdineId { get; set; }
    public string NomeOrdine { get; set; }
    public string ClienteNome { get; set; }
    public string IndirizzoSpedizione { get; set; }
    public string MetodoPagamento { get; set; }
    public string TipoSpedizione { get; set; }
    public decimal CostoSpedizione { get; set; }
    public string StatoOrdine { get; set; }
    public DateTime DataAcquisto { get; set; }
    public List<DettaglioOrdineProdottoViewModel> Prodotti { get; set; } // Corretto tipo di prodotti
    public decimal Subtotale { get; set; }
    public decimal Totale { get; set; }
}