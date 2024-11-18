using System.ComponentModel.DataAnnotations;
public abstract class General
{
    public virtual int Id { get; set; }
    [Required(ErrorMessage = "Il campo Nome è obbligatorio.")]
    public virtual string Nome { get; set; }
}