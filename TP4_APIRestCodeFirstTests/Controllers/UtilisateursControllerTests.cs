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
using System.Reflection;
using System.Text.RegularExpressions;
using static TP4_APIRestCodeFirst.Models.Repository.IDataRepository;
using TP4_APIRestCodeFirst.Models.DataManager;
using Microsoft.EntityFrameworkCore;

namespace TP4_APIRestCodeFirst.Controllers.Tests
{
    [TestClass()]
    public class UtilisateursControllerTests
    {
        private FilmRatingsDBContext context;
        private UtilisateursController controller;
        private IDataRepository<Utilisateur> dataRepository;


        [TestInitialize]
        public void TestInitialize()
        {
            var builder = new DbContextOptionsBuilder<FilmRatingsDBContext>().UseNpgsql();
            context = new FilmRatingsDBContext();
            //controller = new UtilisateursController(context);
            dataRepository = new UtilisateurManager(context);
            controller = new UtilisateursController(dataRepository);

        }

        [TestMethod()]
        public void UtilisateursControllerTest()
        {
            // Assert
            Assert.IsNotNull(context, "Le contexte est nul.");
            Assert.IsNotNull(controller, "Le controller est nul.");
        }

        [TestMethod()]
        public void GetUtilisateursTest()
        {
            // Arrange
            var lesUtilisateurs = context.Utilisateurs.ToList();

            // Act
            var result = controller.GetUtilisateurs().Result;
            var listeUtilisateurs = result.Value.ToList();

            // Assert
            Assert.IsNotNull(result, "Le résultat est nul.");
            Assert.IsInstanceOfType(result, typeof(ActionResult<IEnumerable<Utilisateur>>), "Pas un ActionResult.");
            CollectionAssert.AreEqual(lesUtilisateurs, listeUtilisateurs, "Utilisateurs pas identiques.");
        }

        [TestMethod()]
        public void GetUtilisateurById_ExistingIdPassed_ReturnsRightItem()
        {
            // Arrange
            var utilisateur1 = context.Utilisateurs.Where(u => u.UtilisateurId == 1).FirstOrDefault();

            // Act
            var result = controller.GetUtilisateurById(1).Result;

            // Assert
            Assert.IsNotNull(result, "Aucun résultat.");
            Assert.IsInstanceOfType(result, typeof(ActionResult<Utilisateur>), "Pas un ActionResult.");
            Assert.IsNull(result.Result, "Il y a une erreur.");
            Assert.IsInstanceOfType(result.Value, typeof(Utilisateur), "Pas un Utilisateur");
            Assert.AreEqual(utilisateur1, result.Value, "Utilisateurs pas identiques.");
        }

        [TestMethod()]
        public void GetUtilisateurById_UnknownGuidePassed_ReturnsNotFoundResult()
        {
            // Act
            var result = controller.GetUtilisateurById(0).Result;

            // Assert
            Assert.IsInstanceOfType(result, typeof(ActionResult<Utilisateur>), "Pas un ActionResult.");
            Assert.IsNotNull(result.Result, "Il n'y a pas d'erreur.");
            var errorResult = result.Result as NotFoundResult;
            Assert.AreEqual(errorResult.StatusCode, StatusCodes.Status404NotFound, "Pas une erreur 404.");
            Assert.IsNull(result.Value, "Utilisateur créé alors qu'il y a erreur.");
        }

        [TestMethod()]
        public void GetUtilisateurByEmail_ExistingIdPassed_ReturnsRightItem()
        {
            // Arrange
            var utilisateur1 = context.Utilisateurs.Where(u => u.Mail == "gdominguez0@washingtonpost.com").FirstOrDefault();

            // Act
            var result = controller.GetUtilisateurByEmail("gdominguez0@washingtonpost.com").Result;

            // Assert
            Assert.IsInstanceOfType(result, typeof(ActionResult<Utilisateur>), "Pas un ActionResult.");
            Assert.IsNull(result.Result, "Il y a une erreur.");
            Assert.IsInstanceOfType(result.Value, typeof(Utilisateur), "Pas un utilisateur");
            Assert.AreEqual(utilisateur1, result.Value, "Utilisateurs pas identiques.");
        }

        [TestMethod()]
        public void GetUtilisateurByEmail_UnknownGuidePassed_ReturnsNotFoundResult()
        {
            // Act
            var result = controller.GetUtilisateurByEmail("a").Result;

            // Assert
            Assert.IsInstanceOfType(result, typeof(ActionResult<Utilisateur>), "Pas un ActionResult.");
            Assert.IsNotNull(result.Result, "Il n'y a pas d'erreur.");
            var errorResult = result.Result as NotFoundResult;
            Assert.AreEqual(errorResult.StatusCode, StatusCodes.Status404NotFound, "Pas une erreur 404.");
            Assert.IsNull(result.Value, "Utilisateur créé alors qu'il y a erreur.");
        }

        public void Postutilisateur_ModelValidated_CreationOK()
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
            var result = controller.PostUtilisateur(userAtester).Result;

            // Assert
            Utilisateur? userRecupere = context.Utilisateurs.Where(u => u.Mail.ToUpper() == userAtester.Mail.ToUpper()).FirstOrDefault(); // On récupère l'utilisateur créé directement dans la BD grace à son mail unique
            // On ne connait pas l'ID de l’utilisateur envoyé car numéro automatique.
            // Du coup, on récupère l'ID de celui récupéré et on compare ensuite les 2 users
            userAtester.UtilisateurId = userRecupere.UtilisateurId;
            Assert.AreEqual(userRecupere, userAtester, "Utilisateurs pas identiques");
        }

        [TestMethod()]
        [ExpectedException(typeof(System.AggregateException))]
        public void PostUtilisateur_InvalidInput_ReturnsSystemAggregateException()
        {
            // Arrange
            Utilisateur utilisateurAjoute = new Utilisateur()
            {
                Nom = "Calida",
                Prenom = "Lilley",
                Mobile = "0653930778",
                Pwd = "Toto12345678!",
                Rue = "Impasse des bergeronnettes",
                CodePostal = "74200",
                Ville = "Allinges",
                Pays = "France",
                Latitude = (float)46.344795,
                Longitude = (float)6.4885845
            };

            // Act
            var result = controller.PostUtilisateur(utilisateurAjoute).Result;
        }

        [TestMethod()]
        public void PostUtilisateur_InvalidModel_ReturnsModelError()
        {
            // Arrange
            Utilisateur utilisateur = new Utilisateur()
            {
                Nom = "Calida",
                Prenom = "Lilley",
                Mobile = "0653930778",
                Pwd = "Toto12345678!",
                Rue = "Impasse des bergeronnettes",
                CodePostal = "74200",
                Ville = "Allinges",
                Pays = "France",
                Latitude = (float)46.344795,
                Longitude = (float)6.4885845
            };

            // Assert
            string PwdRegex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*(),.?"":{}|<>]).{12,20}$";
            Regex regex = new Regex(PwdRegex);
            if (!regex.IsMatch(utilisateur.Mobile))
            {
                controller.ModelState.AddModelError("Pwd", "Le mot de passe doit contenir entre 12 et 20 caractères avec 1 lettre majuscule, 1 lettre minuscule, 1 chiffre et 1 caractère spéciale.");
            }
        }

        [TestMethod()]
        public void  PutSerie_ValidUpdate_ReturnsNoContent()
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
                Longitude = (float)6.4885845,
                DateCreation = DateTime.Now.Date
            };

            // Act
            var result = controller.PutUtilisateur(1, utilisateurATester).Result;

            // Assert
            var utilisateur1 = context.Utilisateurs.Where(u => u.UtilisateurId == 1).FirstOrDefault();
            Assert.IsInstanceOfType(result, typeof(NoContentResult), "N'est pas un NoContent");
            Assert.AreEqual(((NoContentResult)result).StatusCode, StatusCodes.Status204NoContent, "N'est pas 204");
            Assert.AreEqual(utilisateur1, utilisateurATester, "L'Utilisateur n'a pas été modifié !");
        }

        [TestMethod()]
        public void PutUtilisateur_InvalidUpdate_ReturnsBadRequest()
        {
            // Arrange
            Utilisateur utilisateur = new Utilisateur()
            {
                UtilisateurId = 1,
                Nom = "Calida",
                Prenom = "Lilley",
                Mobile = "0653930778",
                Mail = "Calida.Lilley@gmail.com",
                Pwd = "Toto12345678!",
                Rue = "Impasse des bergeronnettes",
                CodePostal = "74200",
                Ville = "Allinges",
                Pays = "France",
                Latitude = (float)46.344795,
                Longitude = (float)6.4885845
            };

            // Act
            var result = controller.PutUtilisateur(2, utilisateur).Result;

            // Assert
            Assert.IsInstanceOfType(result, typeof(IActionResult), "N'est pas un IActionResult");
            Assert.AreEqual(((BadRequestResult)result).StatusCode, StatusCodes.Status400BadRequest, "N'est pas une erreur 400 BadRequest");
        }


        [TestMethod()]
        [ExpectedException(typeof(System.AggregateException))]
        public void PutUtilisateur_InvalidInput_ReturnsSystemAggregateException()
        {
            // Arrange
            Utilisateur utilisateur = new Utilisateur()
            {
                UtilisateurId = 1,
                Nom = "Calida",
                Prenom = "Lilley",
                Mobile = "0653930778",
                Pwd = "Toto12345678!",
                Rue = "Impasse des bergeronnettes",
                CodePostal = "74200",
                Ville = "Allinges",
                Pays = "France",
                Latitude = (float)46.344795,
                Longitude = (float)6.4885845
            };

            // Act
            var result = controller.PutUtilisateur(1, utilisateur).Result;
        }

        [TestMethod()]
        public void PutUtilisateur_InvalidModel_ReturnsModelError()
        {
            // Arrange
            Utilisateur utilisateur = new Utilisateur()
            {
                UtilisateurId = 1,
                Nom = "Calida",
                Prenom = "Lilley",
                Mobile = "112",
                Mail = "Calida.Lilley@gmail.com",
                Pwd = "Toto12345678!",
                Rue = "Impasse des bergeronnettes",
                CodePostal = "74200",
                Ville = "Allinges",
                Pays = "France",
                Latitude = (float)46.344795,
                Longitude = (float)6.4885845
            };
            
            // Assert
            string PhoneRegex = @"^0[0-9]{9}$";
            Regex regex = new Regex(PhoneRegex);
            if (!regex.IsMatch(utilisateur.Mobile))
            {
                controller.ModelState.AddModelError("Mobile", "Le numéro de téléphone n'est pas valide."); //On met le même message que dans la classe Utilisateur.
            }
        }

        [TestMethod]
        public void DeleteUtilisateur_ShouldRemoveUser_WhenUserExists()
        {
            // Arrange

            // Ajout d'un utilisateur dans la base de données
            Utilisateur utilisateur = new Utilisateur()
            {
                Nom = "Barrier",
                Prenom = "Florian",
                Mobile = "0612345678",
                Mail = "Florian.Barrier@gmail.com",
                Pwd = "isofghqsiIHD4!",
                Rue = "Impasse des bergeronnettes",
                CodePostal = "74200",
                Ville = "Allinges",
                Pays = "France",
                Latitude = (float)46.344795,
                Longitude = (float)6.4885845
            };
            context.Utilisateurs.Add(utilisateur);
            context.SaveChanges();

            var utilisateurId = utilisateur.UtilisateurId;

            // Act
            var result = controller.DeleteUtilisateur(utilisateurId).Result;

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            var utilisateurSupprime = context.Utilisateurs.Find(utilisateurId);
            Assert.IsNull(utilisateurSupprime);
        }




    }
}