public class Ordine : General
{
    public override string Nome
    {
        get
        {
            if (Id == 0 || Cliente == null)
            {
                return "Ordine-0000"; // in caso di dati incompleti
            }
            return $"BRT-{Id}_{Cliente.Id}";
        }
    }

    // Data in cui è stato effettuato l'acquisto
    public DateTime DataAcquisto { get; set; } = DateTime.Now;

    // Cliente associato all'ordine
    public string ClienteId { get; set; } = string.Empty; // non deve essere nullo
    public Cliente Cliente { get; set; } = null!; // marcato non-nullable

    // Relazione con OrdineDettaglio
    public List<OrdineDettaglio> OrdineDettagli { get; set; } = new List<OrdineDettaglio>();
}
