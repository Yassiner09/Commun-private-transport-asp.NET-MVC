using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Projet.Models
{
    public class GérerDemande
    {
        public int ID { get; set; }

        [Display(Name ="Nombre de demandes")]
        public int nbr { get; set; }

        //[Required(ErrorMessage = "La date de début d'offre est obligatoire.")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = "Date de début")]
        public System.DateTime DateDebut { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = "Date de fin")]
        public System.DateTime DateFin { get; set; }

        //[Required(ErrorMessage = "L'heure de départ est obligatoire.")]
        [DataType(DataType.Time)]
        [Display(Name = "Heure de départ")]
        public System.TimeSpan HeureDep { get; set; }

        //[Required(ErrorMessage = "L'heure d'arrivée est obligatoire.")]
        [DataType(DataType.Time)]
        [Display(Name = "Heure d'arrivée")]
        public System.TimeSpan HeureDarr { get; set; }

        [Display(Name ="Ville de départ")]
        public String IDVilleDep { get; set; }

        [Display(Name = "Ville d'arrivée")]
        public String IDVilleDarr { get; set; }

        
    }
}