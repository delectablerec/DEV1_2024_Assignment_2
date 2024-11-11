public class Prodotto : General
{
    public decimal Prezzo { get; set; } // Prezzo del prodotto
    public int Giacenza { get; set; }   // Quantità disponibile in magazzino
    public string? Colore { get; set; }
    public Categoria? Categoria { get; set; }  // Relazione con la categoria
    public Marca? Marca{get; set;}    
}