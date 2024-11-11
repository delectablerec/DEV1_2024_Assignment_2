public class Orologio : Prodotto
{
    public string Modello{ get; set; }
    public string Referenza{ get; set; }
    public Materiale Materiale { get; set; }
    public Tipologia Tipologia { get; set; }
    public int Diametro { get; set; }
    public Genere Genere {get; set; }
}