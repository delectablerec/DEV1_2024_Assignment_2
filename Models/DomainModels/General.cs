using Microsoft.AspNetCore.Identity;

public abstract class General : IdentityUser
{
    public virtual int Id { get; set; }
    public virtual string Nome { get; set; } = "";
    
}