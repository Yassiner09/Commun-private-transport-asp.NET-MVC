//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle.
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Projet.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Societe
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Societe()
        {
            this.Abonnement = new HashSet<Abonnement>();
            this.Demande = new HashSet<Demande>();
        }
    
        public int IDSociete { get; set; }

        [Required(ErrorMessage ="Le nom est obligatoire."),MinLength(3,ErrorMessage ="Le nom doit contenir au moins 3 caractères.")]
        [Display(Name ="nom de société")]
        [DataType(DataType.Text)]
        public string Nom { get; set; }

        [Required(ErrorMessage = "L'identifiant est obligatoire."), MinLength(7, ErrorMessage = "L'identifiant doit contenir au moins 7 caractères.")]
        [Display(Name = "identifiant de société")]
        [DataType(DataType.Text)]
        public string Identifiant { get; set; }

        [Required(ErrorMessage = "Le mot de passe est obligatoire."), MinLength(7, ErrorMessage = "Le mot de passe doit contenir au moins 7 caractères.")]
        [Display(Name = "mot de passe")]
        [DataType(DataType.Password)]
        public string MotDePasse { get; set; }

        [Display(Name = "vérification du mot de passe")]
        [DataType(DataType.Password)]
        [Compare("MotDePasse", ErrorMessage ="les mots de passe ne sont compatibles.")]
        public string VérifierMotDePasse { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Abonnement> Abonnement { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Demande> Demande { get; set; }
    }
}
