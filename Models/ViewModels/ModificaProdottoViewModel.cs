public class ModificaProdottoViewModel
{
    public Orologio Orologio { get; set; }
    public List<Categoria> Categorie { get; set; }
    public List<Marca> Marche { get; set; }
    public List<Materiale> Materiali { get; set; }
    public List<Tipologia> Tipologie { get; set; }
    public List<Genere> Generi { get; set; }
    public IFormFile ImmagineCaricata { get; set; }
}