public class OrdiniViewModel
{
    public List<Ordine> Ordini { get; set; }

    public Ordine Ordine { get; set; }
    public Cliente Cliente { get; set; }

    public Orologio Orologio {get;set;}

    public string TipoSpedizione{get;set;}

    public decimal CostoSpedizione {get;set;}
    public string MetodoPagamento {get;set;}
    public string Indirizzo{get;set;}

    public DateTime DataAcquisto{get;set;}

    public Carrello Carrello{get;set;}

   // public decimal PrezzoFinale{get;set;}
}
