
namespace TP4_APIRestCodeFirst.Models.EntityFramework
{
    public partial class Notation
    {
        public Notation()
        {
        }

        public Notation(int filmId, int note, Utilisateur utilisateurNotant, Film filmNote)
        {
            FilmId = filmId;
            Note = note;
            UtilisateurNotant = utilisateurNotant;
            FilmNote = filmNote;
        }

        public Notation(int utilisateurId, int filmId, int note, Utilisateur utilisateurNotant, Film filmNote)
        {
            UtilisateurId = utilisateurId;
            FilmId = filmId;
            Note = note;
            UtilisateurNotant = utilisateurNotant;
            FilmNote = filmNote;
        }

        public override bool Equals(object? obj)
        {
            return obj is Notation notation &&
                   UtilisateurId == notation.UtilisateurId &&
                   FilmId == notation.FilmId &&
                   Note == notation.Note &&
                   EqualityComparer<Utilisateur>.Default.Equals(UtilisateurNotant, notation.UtilisateurNotant) &&
                   EqualityComparer<Film>.Default.Equals(FilmNote, notation.FilmNote);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(UtilisateurId, FilmId, Note, UtilisateurNotant, FilmNote);
        }
    }
}
