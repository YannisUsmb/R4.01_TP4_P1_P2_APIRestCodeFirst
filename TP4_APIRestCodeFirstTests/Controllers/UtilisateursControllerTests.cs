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
using Moq;

namespace TP4_APIRestCodeFirst.Controllers.Tests
{
    [TestClass()]
    public class UtilisateursControllerTests
    {
        private FilmRatingsDBContext context;
        private UtilisateursController controller;
        private IDataRepository<Utilisateur> dataRepository;

        private Mock<IDataRepository<Utilisateur>> mockRepository;
        private UtilisateursController controllerMoq;


        [TestInitialize]
        public void TestInitialize()
        {
            var builder = new DbContextOptionsBuilder<FilmRatingsDBContext>().UseNpgsql();
            context = new FilmRatingsDBContext();
            //controller = new UtilisateursController(context);
            dataRepository = new UtilisateurManager(context);
            controller = new UtilisateursController(dataRepository);

            mockRepository = new Mock<IDataRepository<Utilisateur>>();
            controllerMoq = new UtilisateursController(mockRepository.Object);
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
        public void GetUtilisateurById_ExistingIdPassed_ReturnsRightItem_AvecMoq()
        {
            // Arrange
            Utilisateur utilisateur = new Utilisateur
            {
                UtilisateurId = 1,
                Nom = "Calida",
                Prenom = "Lilley",
                Mobile = "0653930778",
                Mail = "clilleymd@last.fm",
                Pwd = "Toto12345678!",
                Rue = "Impasse des bergeronnettes",
                CodePostal = "74200",
                Ville = "Allinges",
                Pays = "France",
                Latitude = 46.344795F,
                Longitude = 6.4885845F,
                DateCreation = DateTime.Now.Date
            };
            mockRepository.Setup(x => x.GetByIdAsync(1).Result).Returns(utilisateur);

            // Act
            var result = controllerMoq.GetUtilisateurById(1).Result;

            // Assert
            Assert.IsNotNull(result, "Aucun résultat.");
            Assert.IsInstanceOfType(result, typeof(ActionResult<Utilisateur>), "Pas un ActionResult.");
            Assert.IsNull(result.Result, "Il y a une erreur.");
            Assert.IsInstanceOfType(result.Value, typeof(Utilisateur), "Pas un Utilisateur");
            Assert.AreEqual(utilisateur, result.Value, "Utilisateurs pas identiques.");
        }

        [TestMethod]
        public void GetUtilisateurById_UnknownIdPassed_ReturnsNotFoundResult_Moq()
        {
            var mockRepository = new Mock<IDataRepository<Utilisateur>>();
            var userController = new UtilisateursController(mockRepository.Object);

            // Act
            var actionResult = userController.GetUtilisateurById(0).Result;

            // Assert
            Assert.IsInstanceOfType(actionResult.Result, typeof(NotFoundResult));
        }

        /*[TestMethod()]
        public void GetUtilisateurById_UnknownIdPassed_ReturnsNotFoundResult()
        {
            // Act
            var result = controller.GetUtilisateurById(0).Result;

            // Assert
            Assert.IsInstanceOfType(result, typeof(ActionResult<Utilisateur>), "Pas un ActionResult.");
            Assert.IsNotNull(result.Result, "Il n'y a pas d'erreur.");
            var errorResult = result.Result as NotFoundResult;
            Assert.AreEqual(errorResult.StatusCode, StatusCodes.Status404NotFound, "Pas une erreur 404.");
            Assert.IsNull(result.Value, "Utilisateur créé alors qu'il y a erreur.");
        }*/

        [TestMethod()]
        public void GetUtilisateurByEmail_ExistingIdPassed_ReturnsRightItem()
        {
            // Arrange
            var utilisateur1 = context.Utilisateurs.Where(u => u.Mail == "clilleymd@last.fm").FirstOrDefault();

            // Act
            var result = controller.GetUtilisateurByEmail("clilleymd@last.fm").Result;

            // Assert
            Assert.IsInstanceOfType(result, typeof(ActionResult<Utilisateur>), "Pas un ActionResult.");
            Assert.IsNull(result.Result, "Il y a une erreur.");
            Assert.IsInstanceOfType(result.Value, typeof(Utilisateur), "Pas un utilisateur");
            Assert.AreEqual(utilisateur1, result.Value, "Utilisateurs pas identiques.");
        }

        [TestMethod()]
        public void GetUtilisateurByEmail_ExistingEmailPassed_ReturnsRightItem_AvecMoq()
        {
            // Arrange
            Utilisateur utilisateur = new Utilisateur
            {
                UtilisateurId = 1,
                Nom = "Calida",
                Prenom = "Lilley",
                Mobile = "0653930778",
                Mail = "clilleymd@last.fm",
                Pwd = "Toto12345678!",
                Rue = "Impasse des bergeronnettes",
                CodePostal = "74200",
                Ville = "Allinges",
                Pays = "France",
                Latitude = 46.344795F,
                Longitude = 6.4885845F,
                DateCreation = DateTime.Now.Date
            };
            mockRepository.Setup(x => x.GetByStringAsync("clilleymd@last.fm").Result).Returns(utilisateur);

            // Act
            var result = controllerMoq.GetUtilisateurByEmail("clilleymd@last.fm").Result;

            // Assert
            Assert.IsNotNull(result, "Aucun résultat.");
            Assert.IsInstanceOfType(result, typeof(ActionResult<Utilisateur>), "Pas un ActionResult.");
            Assert.IsNull(result.Result, "Il y a une erreur.");
            Assert.IsInstanceOfType(result.Value, typeof(Utilisateur), "Pas un Utilisateur");
            Assert.AreEqual(utilisateur, result.Value, "Utilisateurs pas identiques.");
        }

        [TestMethod]
        public void GetUtilisateurByEmail_UnknownEmailPassed_ReturnsNotFoundResult_Moq()
        {
            var mockRepository = new Mock<IDataRepository<Utilisateur>>();
            var userController = new UtilisateursController(mockRepository.Object);

            // Act
            var actionResult = userController.GetUtilisateurByEmail("a").Result;

            // Assert
            Assert.IsInstanceOfType(actionResult.Result, typeof(NotFoundResult));
        }

        /*[TestMethod()]
        public void GetUtilisateurByEmail_UnknownEmailPassed_ReturnsNotFoundResult()
        {
            // Act
            var result = controller.GetUtilisateurByEmail("a").Result;

            // Assert
            Assert.IsInstanceOfType(result, typeof(ActionResult<Utilisateur>), "Pas un ActionResult.");
            Assert.IsNotNull(result.Result, "Il n'y a pas d'erreur.");
            var errorResult = result.Result as NotFoundResult;
            Assert.AreEqual(errorResult.StatusCode, StatusCodes.Status404NotFound, "Pas une erreur 404.");
            Assert.IsNull(result.Value, "Utilisateur créé alors qu'il y a erreur.");
        }*/

        public void Postutilisateur_ModelValidated_CreationOK()
        {
            // Arrange
            Random rnd = new Random();
            int chiffre = rnd.Next(1, 1000000000);

            Utilisateur utilisateurATester = new Utilisateur()
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
            var result = controller.PostUtilisateur(utilisateurATester).Result;

            // Assert
            Utilisateur? utilisateurRecuperer = context.Utilisateurs.Where(u => u.Mail.ToUpper() == utilisateurATester.Mail.ToUpper()).FirstOrDefault();

            utilisateurATester.UtilisateurId = utilisateurRecuperer.UtilisateurId;
            Assert.IsInstanceOfType(result, typeof(ActionResult<Utilisateur>), "Pas un ActionResult<Utilisateur>");
            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult), "Pas un CreatedAtActionResult");

            var createdAtRouteResult = result.Result as CreatedAtActionResult;

            Assert.IsInstanceOfType(result.Value, typeof(Utilisateur), "Pas un Utilisateur");
            Assert.AreEqual(utilisateurATester, utilisateurRecuperer, "Utilisateurs pas identiques");
        }

        public void PostUtilisateur_ModelValidated_CreationOK_moq()
        {
            // Arrange
            Random rnd = new Random();
            int chiffre = rnd.Next(1, 1000000000);

            Utilisateur utilisateurATester = new Utilisateur()
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
            var result = controllerMoq.PostUtilisateur(utilisateurATester).Result;

            // Assert
            Assert.IsInstanceOfType(result, typeof(ActionResult<Utilisateur>), "Pas un ActionResult<Utilisateur>");
            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult), "Pas un CreatedAtActionResult");

            var createdAtRouteResult = result.Result as CreatedAtActionResult;

            Assert.IsInstanceOfType(result.Value, typeof(Utilisateur), "Pas un Utilisateur");
            Assert.AreEqual(utilisateurATester, result.Value, "Utilisateurs pas identiques");
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
        public void PutUtilisateur_ValidUpdate_ReturnsNoContent()
        {
            // Arrange
            Random rnd = new Random();
            int chiffre = rnd.Next(1, 1000000000);

            Utilisateur utilisateurATester = new Utilisateur()
            {
                UtilisateurId = 2,
                Nom = "Gwendolin",
                Prenom = "Dominguez",
                Mobile = "0724970555",
                Mail = "machin" + chiffre + "@gmail.com",
                Pwd = "Toto12345678!",
                Rue = "Chemin de gom",
                CodePostal = "73420",
                Ville = "Voglans",
                Pays = "France",
                Latitude = (float)45.622154,
                Longitude = (float)5.8853216,
                DateCreation = DateTime.Now.Date
            };

            // Act
            var result = controller.PutUtilisateur(2, utilisateurATester).Result;

            // Assert
            var utilisateur1 = context.Utilisateurs.Where(u => u.UtilisateurId == 2).FirstOrDefault();
            Assert.IsInstanceOfType(result, typeof(NoContentResult), "N'est pas un NoContent");
            Assert.AreEqual(((NoContentResult)result).StatusCode, StatusCodes.Status204NoContent, "N'est pas 204");
            Assert.AreEqual(utilisateur1, utilisateurATester, "L'Utilisateur n'a pas été modifié !");
        }

        [TestMethod()]
        public void PutUtilisateur_ValidUpdate_ReturnsNoContent_Moq()
        {
            // Arrange
            Random rnd = new Random();
            int chiffre = rnd.Next(1, 1000000000);

            Utilisateur utilisateurAvant = new Utilisateur()
            {
                UtilisateurId = 3,
                Nom = "Randolph",
                Prenom = "Richings",
                Mobile = "0630271158",
                Mail = "rrichings1@naver.com",
                Pwd = "Toto12345678!",
                Rue = "Route des charmottes d'en bas",
                CodePostal = "74890",
                Ville = "Bons-en-Chablais",
                Pays = "France",
                Latitude = (float)46.25732,
                Longitude = (float)6.367676,
                DateCreation = DateTime.Now.Date
            };

            Utilisateur utilisateurApres = new Utilisateur()
            {
                UtilisateurId = 3,
                Nom = "Randolph",
                Prenom = "Richings",
                Mobile = "0630271158",
                Mail = "a@naver.com", // email modifié
                Pwd = "Toto12345678!",
                Rue = "Route des charmottes d'en bas",
                CodePostal = "74890",
                Ville = "Bons-en-Chablais",
                Pays = "France",
                Latitude = (float)46.25732,
                Longitude = (float)6.367676,
                DateCreation = DateTime.Now.Date
            };
            mockRepository.Setup(x => x.GetByIdAsync(3).Result).Returns(utilisateurAvant);

            // Act
            var result = controllerMoq.PutUtilisateur(3, utilisateurApres).Result;

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult), "N'est pas un NoContent");
            Assert.AreEqual(((NoContentResult)result).StatusCode, StatusCodes.Status204NoContent, "N'est pas 204");
            Assert.AreEqual(((OkObjectResult)result).Value, utilisateurApres, "L'Utilisateur n'a pas été modifié !");
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
        public void DeleteUtilisateurTest_OK()
        {
            // Arrange
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

        [TestMethod]
        public void DeleteUtilisateurTest_OK_AvecMoq()
        {
            // Arrange
            Utilisateur utilisateur = new Utilisateur
            {
                UtilisateurId = 1,
                Nom = "Calida",
                Prenom = "Lilley",
                Mobile = "0653930778",
                Mail = "clilleymd@last.fm",
                Pwd = "Toto12345678!",
                Rue = "Impasse des bergeronnettes",
                CodePostal = "74200",
                Ville = "Allinges",
                Pays = "France",
                Latitude = 46.344795F,
                Longitude = 6.4885845F
            };
            mockRepository.Setup(x => x.GetByIdAsync(1).Result).Returns(utilisateur);

            // Act
            var actionResult = controllerMoq.DeleteUtilisateur(1).Result;

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult), "Pas un NoContentResult"); // Test du type de retour
        }




    }
}