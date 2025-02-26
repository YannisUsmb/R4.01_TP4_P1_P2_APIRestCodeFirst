using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TP4_APIRestCodeFirst.Models.DataManager;
using TP4_APIRestCodeFirst.Models.EntityFramework;
using static TP4_APIRestCodeFirst.Models.Repository.IDataRepository;

namespace TP4_APIRestCodeFirst.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtilisateursController : ControllerBase
    {
        private readonly UtilisateurManager utilisateurManager;
        private readonly IDataRepository<Utilisateur> dataRepository;
        //private readonly FilmRatingsDBContext _context;

        /// <summary>
        /// Constructeur du controller
        /// </summary>
        public UtilisateursController(IDataRepository<Utilisateur> dataRepo)
        {
            dataRepository = dataRepo;
        }
        /*public UtilisateursController(dataRepository userManager)
        {
            dataRepository = userManager;
        }
        public UtilisateursController(FilmRatingsDBContext context)
        {
            _context = context;
        }*/

        /// <summary>
        /// Récupère (get) tous les utilisateurs
        /// </summary>
        /// <returns>Réponse http</returns>
        /// <response code="200">Quand tous les utilisateurs ont été renvoyés avec succès</response>
        /// <response code="500">Quand il y a une erreur de serveur interne</response>
        // GET: api/Utilisateurs/GetUtilisateurs
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Utilisateur>>> GetUtilisateurs()
        {
            return await dataRepository.GetAllAsync();
            //return await _context.Utilisateurs.ToListAsync();
        }

        /// <summary>
        /// Récupère (get) un utilisateur par son ID
        /// </summary>
        /// <param name="id">L'id de l'utilisateur</param>
        /// <returns>Réponse http</returns>
        /// <response code="200">Quand l'utilisateur a été trouvé</response>
        /// <response code="404">Quand l'utilisateur n'a pas été trouvé</response>
        /// <response code="500">Quand il y a une erreur de serveur interne</response>
        // GET: api/Utilisateurs/GetUtilisateurById/{id}
        [HttpGet("[action]/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Utilisateur>> GetUtilisateurById(int id)
        {
            var utilisateur = await dataRepository.GetByIdAsync(id);
            //var utilisateur = await _context.Utilisateurs.FindAsync(id);

            if (utilisateur == null)
            {
                return NotFound();
            }

            return utilisateur;
        }

        /// <summary>
        /// Récupère (get) un utilisateur par son email
        /// </summary>
        /// <param name="email">L'email de l'utilisateur</param>
        /// <returns>Réponse http</returns>
        /// <response code="200">Quand l'utilisateur a été trouvé</response>
        /// <response code="404">Quand l'utilisateur n'a pas été trouvé</response>
        /// <response code="500">Quand il y a une erreur de serveur interne</response>
        // GET: api/Utilisateurs/GetUtilisateurByEmail/{email}
        [HttpGet("[action]/{email}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Utilisateur>> GetUtilisateurByEmail(string email)
        {
            var utilisateur = await dataRepository.GetByStringAsync(email);
            //var utilisateur = await _context.Utilisateurs.FirstOrDefaultAsync((d) => d.Mail == email);

            if (utilisateur == null)
            {
                return NotFound();
            }

            return utilisateur;
        }

        /// <summary>
        /// Modifie (put) un utilisateur
        /// </summary>
        /// <param name="id">L'id de l'utilisateur à modifier</param>
        /// <param name="utilisateur">L'utilisateur modifié</param>
        /// <returns>Réponse http</returns>
        /// <response code="204">Quand l'utilisateur a été modifié avec succès</response>
        /// <response code="400">Quand l'id ne correspond pas ou que le format de l'utilisateur est incorrect</response>
        /// <response code="404">Quand l'utilisateur n'a pas été trouvé</response>
        /// <response code="500">Quand il y a une erreur de serveur interne</response>
        // PUT: api/Utilisateurs/PutUtilisateur/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutUtilisateur(int id, Utilisateur utilisateur)
        {
            if (id != utilisateur.UtilisateurId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userToUpdate = await dataRepository.GetByIdAsync(id);
            if (userToUpdate == null)
            {
                return NotFound();
            }
            else
            {
                await dataRepository.UpdateAsync(userToUpdate.Value, utilisateur);
                return NoContent();
            }


            /*
            _context.Entry(utilisateur).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UtilisateurExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
            */
        }

        /// <summary>
        /// Crée (post) un nouvel utilisateur
        /// </summary>
        /// <param name="utilisateur">L'utilisateur à créer</param>
        /// <returns>Réponse http</returns>
        /// <response code="201">Quand l'utilisateur a été créé avec succès</response>
        /// <response code="400">Quand le format de l'utilisateur dans le corps de la requête est incorrect</response>
        /// <response code="500">Quand il y a une erreur de serveur interne</response>
        // POST: api/Utilisateurs/PostUtilisateur
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Utilisateur>> PostUtilisateur(Utilisateur utilisateur)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await dataRepository.AddAsync(utilisateur);
            //_context.Utilisateurs.Add(utilisateur);
            //await _context.SaveChangesAsync();

            return CreatedAtAction("GetUtilisateurById", new { id = utilisateur.UtilisateurId }, utilisateur);
        }

        /// <summary>
        /// Supprime (delete) un utilisateur
        /// </summary>
        /// <param name="id">L'id de l'utilisateur à supprimer</param>
        /// <returns>Réponse http</returns>
        /// <response code="204">Quand l'utilisateur a été supprimé avec succès</response>
        /// <response code="404">Quand l'utilisateur n'a pas été trouvé</response>
        /// <response code="500">Quand il y a une erreur de serveur interne</response>
        // DELETE: api/Utilisateurs/DeleteUtilisateur/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUtilisateur(int id)
        {
            var utilisateur = await dataRepository.GetByIdAsync(id);
            //var utilisateur = await _context.Utilisateurs.FindAsync(id);
            if (utilisateur == null)
            {
                return NotFound();
            }

            await dataRepository.DeleteAsync(utilisateur.Value);
            //_context.Utilisateurs.Remove(utilisateur);
            //await _context.SaveChangesAsync();

            return NoContent();
        }

        /*private bool UtilisateurExists(int id)
        {
            return _context.Utilisateurs.Any(e => e.UtilisateurId == id);
        }*/
    }
}
