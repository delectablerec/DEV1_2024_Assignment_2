using Microsoft.AspNetCore.Identity;
public class Cliente : IdentityUser
{
    public string Nome { get; set; }
}