using System.ComponentModel.DataAnnotations;

namespace GestionOrdinateurs.Shared;

// Classe représentant un utilisateur de l'application
public class User
{
    // Identifiant unique de l'utilisateur
    public int Id { get; set; }

    // Login obligatoire de l'utilisateur
    [Required]
    public string Login { get; set; } = string.Empty;

    // Rôle de l'utilisateur (Gestionnaire ou Admin)
    public RoleEnum Role { get; set; }
}