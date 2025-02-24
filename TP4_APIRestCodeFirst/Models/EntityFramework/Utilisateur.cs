using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TP4_APIRestCodeFirst.Models.EntityFramework
{
    [Table("t_e_utilisateur_utl")]
    [Index(nameof(Mail), IsUnique = true, Name = "uq_utilisateur_mail")]
    public class Utilisateur
    {
        [Key]
        [Column("utl_id")]
        public int UtilisateurId { get; set; }

        [Column("utl_nom", TypeName = "varchar(50)")]
        [StringLength(50)]
        public string? Nom { get; set; }

        [Column("utl_prenom", TypeName = "varchar(50)")]
        [StringLength(50)]
        public string? Prenom { get; set; }

        [Column("utl_mobile", TypeName = "char(10)")]
        [StringLength(10)]
        public string? Mobile { get; set; }

        [Required]
        [Column("utl_mail", TypeName = "varchar(100)")]
        [StringLength(100)]
        public string Mail { get; set; } = null!;

        [Required]
        [Column("utl_pwd", TypeName = "varchar(64)")]
        [StringLength(64)]
        public string Pwd { get; set; } = null!;

        [Column("utl_rue", TypeName = "varchar(200)")]
        [StringLength(200)]
        public string? Rue { get; set; }

        [Column("utl_cp", TypeName = "char(5)")]
        [StringLength(5)]
        public string? CodePostal { get; set; }

        [Column("utl_ville", TypeName = "varchar(50)")]
        [StringLength(50)]
        public string? Ville { get; set; }

        [Column("utl_pays", TypeName = "varchar(50) default 'France'")]
        [StringLength(50)]
        public string? Pays { get; set; }

        [Column("utl_latitude", TypeName = "real")]
        public float? Latitude { get; set; }

        [Column("utl_longitude", TypeName = "real")]
        public float? Longitude { get; set; }

        [Required]
        [Column("utl_datecreation", TypeName = "date default now()")]
        [StringLength(50)]
        public DateTime DateCreation { get; set; }

        [InverseProperty(nameof(Notation.UtilisateurNotant))]
        public virtual ICollection<Notation> NotesUtilisateur { get; set; } = new List<Notation>();
    }
}
