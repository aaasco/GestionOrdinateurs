using GestionOrdinateurs.API.Services;
using GestionOrdinateurs.API;
using GestionOrdinateurs.Shared;
using Microsoft.EntityFrameworkCore;

namespace GestionOrdinateurs.Tests;

// Tests unitaires pour la classe OrdinateurService
public class OrdinateurServiceTest
{
    // Crée une base de données en mémoire pour les tests
    private ParcInformatiqueContext CreerContexteEnMemoire()
    {
        var options = new DbContextOptionsBuilder<ParcInformatiqueContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ParcInformatiqueContext(options);
    }

    [Fact]
    public async Task AjouterOrdinateur_DoitAjouterUnOrdinateur()
    {
        //Arrange : création du service et d'un ordinateur à ajouter
        ParcInformatiqueContext context = CreerContexteEnMemoire();
        OrdinateurService service = new OrdinateurService(context, new JournalService());

        Ordinateur ordinateur = new Ordinateur
        {
            Id = 10,
            Nom = "Ordinateur Test",
            Caracteristiques = new Caracteristiques(16, 512, TypeOrdinateur.Portable)
            {
                Marque = "Marque Test",
                Processeur = "Processeur Test"
            }

        };


        //Act : ajout de l'ordinateur au service
        await service.AjouterOrdinateur(ordinateur);

        //Assert : vérification que l'ordinateur a bien été ajouté
        Assert.Contains(ordinateur, await service.GetOrdinateurs());
    }

    [Fact]
    public async Task SupprimerOrdinateur_DoitSupprimerUnOrdinateur()
    {
        //Arrange : création du service et d'un ordinateur à ajouter
        ParcInformatiqueContext context = CreerContexteEnMemoire();
        OrdinateurService service = new OrdinateurService(context, new JournalService());

        Ordinateur ordinateur = new Ordinateur
        {
            Id = 20,
            Nom = "Ordinateur Test",
            Caracteristiques = new Caracteristiques(16, 512, TypeOrdinateur.Portable)
            {
                Marque = "Marque Test",
                Processeur = "Processeur Test"
            }
        };

        await service.AjouterOrdinateur(ordinateur);

        //Act : suppression de l'ordinateur du service
        bool resultat = await service.SupprimerOrdinateur(ordinateur.Id);

        //Assert : vérification que l'ordinateur a bien été supprimé
        Assert.True(resultat);
        Assert.Null(await service.GetOrdinateurById(ordinateur.Id));
    }

    [Fact]
    public async Task SupprimerOrdinateur_DoitRetournerFalseSiOrdinateurNonExistant()
    {
        //Arrange : création du service
        ParcInformatiqueContext context = CreerContexteEnMemoire();
        OrdinateurService service = new OrdinateurService(context, new JournalService());

        //Act : tentative de suppression d'un ordinateur inexistant
        bool resultat = await service.SupprimerOrdinateur(999);

        //Assert : vérification que la suppression a échoué
        Assert.False(resultat);
    }
}