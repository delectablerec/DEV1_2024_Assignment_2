using Microsoft.AspNetCore.Identity;
public class Cliente : General
{
    public string ImmagineProfiloURL { get; set; } // URL per la pagina profilo --> = "/images/default-profile.png"; // Per assegnare immagine standard
    public string Bio { get; set; } // Descrizione bio
}