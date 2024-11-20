using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;



public class ApplicationDbContext : IdentityDbContext<Cliente>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<CarouselImage> CarouselImages { get; set; }
    public DbSet<Prodotto> Prodotti { get; set; }
    public DbSet<Orologio> Orologi { get; set; }
    public DbSet<Categoria> Categorie { get; set; }
    public DbSet<Marca> Marche { get; set; }
    public DbSet<Materiale> Materiali { get; set; }
    public DbSet<Tipologia> Tipologie { get; set; }
    public DbSet<Genere> Generi { get; set; }

    public DbSet<Ordine> Ordini { get; set; }

    public DbSet<Cliente> Clienti { get; set; }


/*  !!! PER CREARE PRODOTTI E OROLOGI SEPARATI
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure TPT
        modelBuilder.Entity<Prodotto>().ToTable("Prodotti");
        modelBuilder.Entity<Orologio>().ToTable("Orologi");
    }
*/
}
