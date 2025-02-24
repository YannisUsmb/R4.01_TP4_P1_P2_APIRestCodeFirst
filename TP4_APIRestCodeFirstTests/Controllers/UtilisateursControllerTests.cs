using Microsoft.VisualStudio.TestTools.UnitTesting;
using TP4_APIRestCodeFirst.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TP4_APIRestCodeFirst.Models.EntityFramework;
using Microsoft.AspNetCore.Mvc;
using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Http;
using System.Xml;

namespace TP4_APIRestCodeFirst.Controllers.Tests
{
    [TestClass()]
    public class UtilisateursControllerTests
    {
        private FilmRatingsDBContext context;
        private UtilisateursController controller;

        [TestInitialize]
        public void TestInitialize()
        {
            context = new FilmRatingsDBContext();
            controller = new UtilisateursController(context);
        }

        [TestMethod()]
        public void UtilisateursControllerTest()
        {
            Assert.IsNotNull(context, "Le contexte est nul.");
            Assert.IsNotNull(controller, "Le controller est nul.");
        }

        [TestMethod()]
        public void GetUtilisateursTest()
        {
            // Act
            var lesUtilisateurs  = context.Utilisateurs.ToList();

            var result = controller.GetUtilisateurs().Result;
            var listeUtilisateurs = result.Value.ToList();

            // Assert
            Assert.IsNotNull(result, "Le résultat est nul.");
            Assert.IsInstanceOfType(result, typeof(ActionResult<IEnumerable<Utilisateur>>), "Pas un ActionResult.");
            CollectionAssert.AreEqual(lesUtilisateurs, listeUtilisateurs, "Utilisateurs pas identiques.");
        }

        [TestMethod()]
        public async Task GetUtilisateurById_ExistingIdPassed_ReturnsRightItem()
        {
            // Act
            var utilisateur1 = context.Utilisateurs.Where(u => u.UtilisateurId == 1).FirstOrDefault();

            var result = await controller.GetUtilisateurById(1);
            var value = result.Value;

            // Assert
            Assert.IsNotNull(result, "Aucun résultat.");
            Assert.IsInstanceOfType(result, typeof(ActionResult<Utilisateur>), "Pas un ActionResult.");
            Assert.IsNull(result.Result, "Il y a une erreur.");
            Assert.IsInstanceOfType(value, typeof(Utilisateur), "Pas un Utilisateur");
            Assert.AreEqual(utilisateur1, value, "Utilisateurs pas identiques.");
        }

        [TestMethod()]
        public async Task GetUtilisateurById_UnknownGuidePassed_ReturnsNotFoundResult()
        {
            // Act
            var result = await controller.GetUtilisateurById(0);
            var errorResult = result.Result as NotFoundResult;

            // Assert
            Assert.IsInstanceOfType(result, typeof(ActionResult<Utilisateur>), "Pas un ActionResult.");
            Assert.IsNotNull(result.Result, "Il n'y a pas d'erreur.");
            Assert.AreEqual(errorResult.StatusCode, StatusCodes.Status404NotFound, "Pas une erreur 404.");
            Assert.IsNull(result.Value, "Utilisateur créé alors qu'il y a erreur.");
        }

        [TestMethod()]
        public async Task GetUtilisateurByEmail_ExistingIdPassed_ReturnsRightItem()
        {
            // Act
            var utilisateur1 = context.Utilisateurs.Where(u => u.Mail == "gdominguez0@washingtonpost.com").FirstOrDefault();

            var result = await controller.GetUtilisateurByEmail("gdominguez0@washingtonpost.com");
            var value = result.Value;

            // Assert
            Assert.IsInstanceOfType(result, typeof(ActionResult<Utilisateur>), "Pas un ActionResult.");
            Assert.IsNull(result.Result, "Il y a une erreur.");
            Assert.IsInstanceOfType(value, typeof(Utilisateur), "Pas un utilisateur");
            Assert.AreEqual(utilisateur1, value, "Utilisateurs pas identiques.");
        }

        [TestMethod()]
        public async Task GetUtilisateurByEmail_UnknownGuidePassed_ReturnsNotFoundResult()
        {
            // Act
            var result = await controller.GetUtilisateurByEmail("a");
            var errorResult = result.Result as NotFoundResult;

            // Assert
            Assert.IsInstanceOfType(result, typeof(ActionResult<Utilisateur>), "Pas un ActionResult.");
            Assert.IsNotNull(result.Result, "Il n'y a pas d'erreur.");
            Assert.AreEqual(errorResult.StatusCode, StatusCodes.Status404NotFound, "Pas une erreur 404.");
            Assert.IsNull(result.Value, "Utilisateur créé alors qu'il y a erreur.");
        }

        public async Task Postutilisateur_ModelValidated_CreationOK()
        {
            // Arrange
            Random rnd = new Random();
            int chiffre = rnd.Next(1, 1000000000);
            // Le mail doit être unique donc 2 possibilités :
            // 1. on s'arrange pour que le mail soit unique en concaténant un random ou un timestamp
            // 2. On supprime le user après l'avoir créé. Dans ce cas, nous avons besoin d'appeler la méthode DELETE de l’API ou remove du DbSet.
            Utilisateur userAtester = new Utilisateur()
            {
                Nom = "MACHIN",
                Prenom = "Luc",
                Mobile = "0606070809",
                Mail = "machin" + chiffre + "@gmail.com",
                Pwd = "Toto1234!",
                Rue = "Chemin de Bellevue",
                CodePostal = "74940",
                Ville = "Annecy-le-Vieux",
                Pays = "France",
                Latitude = null,
                Longitude = null
            };

            // Act
            var result = await controller.PostUtilisateur(userAtester);

            // Assert
            Utilisateur? userRecupere = context.Utilisateurs.Where(u => u.Mail.ToUpper() == userAtester.Mail.ToUpper()).FirstOrDefault(); // On récupère l'utilisateur créé directement dans la BD grace à son mail unique
            // On ne connait pas l'ID de l’utilisateur envoyé car numéro automatique.
            // Du coup, on récupère l'ID de celui récupéré et on compare ensuite les 2 users
            userAtester.UtilisateurId = userRecupere.UtilisateurId;
            Assert.AreEqual(userRecupere, userAtester, "Utilisateurs pas identiques");
        }

        [TestMethod]
        public async Task PutSerie_ValidUpdate_ReturnsNoContent()
        {
            // Arrange
            Random rnd = new Random();
            int chiffre = rnd.Next(1, 1000000000);

            Utilisateur utilisateurATester = new Utilisateur()
            {
                UtilisateurId = 1,
                Nom = "Calida",
                Prenom = "Lilley",
                Mobile = "0653930778",
                Mail = "machin" + chiffre + "@gmail.com",
                Pwd = "Toto12345678!",
                Rue = "Impasse des bergeronnettes",
                CodePostal = "74200",
                Ville = "Allinges",
                Pays = "France",
                Latitude = (float)46.344795,
                Longitude = (float)6.4885845
            };

            // Act
            var result = await controller.PutUtilisateur(1, utilisateurATester);

            // Assert
            var utilisateur1 = context.Utilisateurs.Where(u => u.UtilisateurId == 1).FirstOrDefault();
            Assert.IsInstanceOfType(result, typeof(NoContentResult), "N'est pas un NoContent");
            Assert.AreEqual(((NoContentResult)result).StatusCode, StatusCodes.Status204NoContent, "N'est pas 204");
            Assert.AreEqual((Utilisateur)utilisateurATester, utilisateur1, "L'Utilisateur n'a pas été modifié !");
        }
    }
}