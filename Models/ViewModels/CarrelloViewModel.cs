public class CarrelloViewModel
{
    public List<Orologio> Carrello { get; set; } = new List<Orologio>();
    public decimal Totale { get; set; } = 0;
    public int Quantita { get; set; } = 0;
}