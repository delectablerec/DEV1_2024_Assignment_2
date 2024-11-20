//Permette di gestire i prodotti aggiunti al carrello in modo dinamico senza doverli immediatamente salvare nel database
//funge da base per ordine finale

public class OrologioInCarrello
{
    // Identificativo univoco dell'orologio
    public int OrologioId { get; set; } 

    // Riferimento all'orologio aggiunto al carrello
    public Orologio Orologio { get; set; } 

     // Numero di unità dell' orologio nel carrello
    public int QuantitaInCarrello { get; set; } 
}