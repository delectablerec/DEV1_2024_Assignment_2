public class Prodotto : General
{
    public  decimal Prezzo { get; set; } // Prezzo del prodotto
    public int Giacenza { get; set; }   // Quantità disponibile in magazzino
    public string Colore { get; set; }
    public string UrlImmagine { get; set; }
    public int CategoriaId { get; set; } 
    public Categoria Categoria { get; set; }  // Relazione con la categoria
    public int MarcaId { get; set; }
    public Marca Marca {get; set;}
    // public string? Descrizione { get; set; } 
}