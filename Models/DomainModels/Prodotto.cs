using System.ComponentModel.DataAnnotations;
public class Prodotto : General
{
    //[Required(ErrorMessage = "Il campo Prezzo è obbligatorio.")]
    public  decimal Prezzo { get; set; } // Prezzo del prodotto
    //[Required(ErrorMessage = "Il campo Giacenza è obbligatorio.")]
    public int Giacenza { get; set; }   // Quantità disponibile in magazzino
    //[Required(ErrorMessage = "Il campo Colore è obbligatorio.")]
    public string Colore { get; set; }
    //[Required(ErrorMessage = "Il campo UrlImmagine è obbligatorio.")]
    public string UrlImmagine { get; set; } // Percorso dell'immagine
    
    //[Required(ErrorMessage = "Il campo Categoria è obbligatorio.")]
    public int CategoriaId { get; set; } 
    public Categoria Categoria { get; set; }  // Relazione con la categoria
    //[Required(ErrorMessage = "Il campo Marca è obbligatorio.")]
    public int MarcaId { get; set; }
    public Marca Marca {get; set;}
    // public string? Descrizione { get; set; } 
}