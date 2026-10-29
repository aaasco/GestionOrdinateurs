using GestionOrdinateurs.Shared;

namespace GestionOrdinateurs.Tests;

// Tests unitaires pour la classe Reparation
public class ReparationTest
{
    [Fact]
    public void CreerReparation_DoitCreerUneReparation()
    {
        //Arrange : création d'un ordinateur
        Ordinateur ordinateur = new Ordinateur
        {
            Id = 1,
            Nom = "Ordinateur Test",
            Caracteristiques = new Caracteristiques(16, 512, TypeOrdinateur.Portable)
        };

        //Act : création d'une réparation associée à l'ordinateur
        Reparation reparation = new Reparation
        {
            Id = 1,
            UnOrdinateur = ordinateur,
            TypeReparation = "Changement SSD",
            DateDebut = DateTime.Now,
            Statut = "En cours",
            Technicien = "Technicien 1"
        };

        //Assert : vérification que la réparation a bien été créée
        Assert.Equal(1, reparation.Id);
        Assert.Equal(ordinateur, reparation.UnOrdinateur);
        Assert.Equal("Changement SSD", reparation.TypeReparation);
        Assert.Equal("En cours", reparation.Statut);
        Assert.Equal("Technicien 1", reparation.Technicien);
    }
}