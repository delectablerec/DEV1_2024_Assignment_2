using System.ComponentModel.DataAnnotations;

public class Orologio : Prodotto
{
    [Required(ErrorMessage = "Il campo Modello è obbligatorio.")]
    public string Modello { get; set; }

    [Required(ErrorMessage = "Il campo Referenza è obbligatorio.")]
    public string Referenza { get; set; }

    [Required(ErrorMessage = "Il campo Materiale è obbligatorio.")]
    public int MaterialeId { get; set; }
    public Materiale Materiale { get; set; }

    public int? TipologiaId { get; set; }
    public Tipologia Tipologia { get; set; }

    [Required(ErrorMessage = "Il campo Diametro è obbligatorio.")]
    [Range(1, 25, ErrorMessage = "Il diametro deve essere tra 1 e 25.")]
    public int Diametro { get; set; }

    [Required(ErrorMessage = "Il campo Genere è obbligatorio.")]
    public int GenereId { get; set; }
    public Genere Genere { get; set; }
}