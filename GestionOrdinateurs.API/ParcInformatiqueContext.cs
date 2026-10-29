using GestionOrdinateurs.Shared;
using Microsoft.EntityFrameworkCore;

namespace GestionOrdinateurs.API;
public class ParcInformatiqueContext : DbContext
{
        public DbSet<Ordinateur> Ordinateurs { get; set; }
        public DbSet<Reparation> Reparations { get; set; }
        public DbSet<User> Users { get; set; }

    // Constructeur permettant de configurer le contexte avec des options spécifiques
    public ParcInformatiqueContext(DbContextOptions<ParcInformatiqueContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Définition de la clé primaire pour l'entité Ordinateur
        modelBuilder.Entity<Ordinateur>().HasKey(o => o.Id);

        // Les caractéristiques appartiennent à l'ordinateur
        modelBuilder.Entity<Ordinateur>().OwnsOne(o => o.Caracteristiques);

        // Un ordinateur peut avoir plusieurs réparations
        modelBuilder.Entity<Ordinateur>()
            .HasMany(o => o.Reparations)
            .WithOne(r => r.UnOrdinateur)
            .HasForeignKey("OrdinateurId");

        // Le login de chaque utilisateur doit être unique
        modelBuilder.Entity<User>().HasIndex(u => u.Login).IsUnique();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Utilisation de SQLite si aucune configuration n'est fournie, avec une chaîne de connexion par défaut
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=parcinfo.db"); //chaîne de connexion à la BDD
        }
    }

    


}