namespace TP4_APIRestCodeFirst.Models.EntityFramework
{
    public partial class Film
    {
        public Film()
        {
        }

        public Film(string titre, string? resume, DateTime? dateSortie, decimal? duree, string? genre, ICollection<Notation> notesFilm)
        {
            Titre = titre;
            Resume = resume;
            DateSortie = dateSortie;
            Duree = duree;
            Genre = genre;
            NotesFilm = notesFilm;
        }

        public Film(int filmId, string titre, string? resume, DateTime? dateSortie, decimal? duree, string? genre, ICollection<Notation> notesFilm)
        {
            FilmId = filmId;
            Titre = titre;
            Resume = resume;
            DateSortie = dateSortie;
            Duree = duree;
            Genre = genre;
            NotesFilm = notesFilm;
        }

        public override bool Equals(object? obj)
        {
            return obj is Film film &&
                   FilmId == film.FilmId &&
                   Titre == film.Titre &&
                   Resume == film.Resume &&
                   DateSortie == film.DateSortie &&
                   Duree == film.Duree &&
                   Genre == film.Genre &&
                   EqualityComparer<ICollection<Notation>>.Default.Equals(NotesFilm, film.NotesFilm);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FilmId, Titre, Resume, DateSortie, Duree, Genre, NotesFilm);
        }
    }
}
