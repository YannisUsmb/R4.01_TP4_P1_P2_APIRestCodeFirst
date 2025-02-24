
namespace TP4_APIRestCodeFirst.Models.EntityFramework
{
    public partial class Utilisateur
    {
        public Utilisateur()
        {
        }

        public Utilisateur(int utilisateurId, string? nom, string? prenom, string? mobile, string mail, string pwd, string? rue, string? codePostal, string? ville, string? pays, float? latitude, float? longitude, DateTime dateCreation, ICollection<Notation> notesUtilisateur)
        {
            UtilisateurId = utilisateurId;
            Nom = nom;
            Prenom = prenom;
            Mobile = mobile;
            Mail = mail;
            Pwd = pwd;
            Rue = rue;
            CodePostal = codePostal;
            Ville = ville;
            Pays = pays;
            Latitude = latitude;
            Longitude = longitude;
            DateCreation = dateCreation;
            NotesUtilisateur = notesUtilisateur;
        }

        public override bool Equals(object? obj)
        {
            return obj is Utilisateur utilisateur &&
                   UtilisateurId == utilisateur.UtilisateurId &&
                   Nom == utilisateur.Nom &&
                   Prenom == utilisateur.Prenom &&
                   Mobile == utilisateur.Mobile &&
                   Mail == utilisateur.Mail &&
                   Pwd == utilisateur.Pwd &&
                   Rue == utilisateur.Rue &&
                   CodePostal == utilisateur.CodePostal &&
                   Ville == utilisateur.Ville &&
                   Pays == utilisateur.Pays &&
                   Latitude == utilisateur.Latitude &&
                   Longitude == utilisateur.Longitude &&
                   DateCreation == utilisateur.DateCreation &&
                   EqualityComparer<ICollection<Notation>>.Default.Equals(NotesUtilisateur, utilisateur.NotesUtilisateur);
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(UtilisateurId);
            hash.Add(Nom);
            hash.Add(Prenom);
            hash.Add(Mobile);
            hash.Add(Mail);
            hash.Add(Pwd);
            hash.Add(Rue);
            hash.Add(CodePostal);
            hash.Add(Ville);
            hash.Add(Pays);
            hash.Add(Latitude);
            hash.Add(Longitude);
            hash.Add(DateCreation);
            hash.Add(NotesUtilisateur);
            return hash.ToHashCode();
        }
    }
}
