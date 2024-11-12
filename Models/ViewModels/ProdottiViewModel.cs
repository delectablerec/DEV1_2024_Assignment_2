public class ProdottiViewModel
{
    public List<Orologio> Orologi { get; set; }
    public List<Categoria> Categorie { get; set; }
    public List<Marca> Marche { get; set; }
    public List<Materiale> Materiali { get; set; }
    public List<Tipologia> Tipologie { get; set; }
    public decimal MinPrezzo { get; set; }
    public decimal MaxPrezzo { get; set; }
    public int NumeroPagine { get; set; }
    public int PaginaCorrente { get; set; }
}
