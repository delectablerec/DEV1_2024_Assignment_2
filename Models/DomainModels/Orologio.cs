public class Orologio : Prodotto
{
    public string Modello{ get; set; }
    public string Referenza{ get; set; }
    public int MaterialeId { get; set; }
    public Materiale Materiale { get; set; }
    public int TipologiaId { get; set; }
    public Tipologia Tipologia { get; set; }
    public int DiametroId { get; set; }
    public int Diametro { get; set; }
    public int GenereId { get; set; }
    public Genere Genere {get; set; }
}