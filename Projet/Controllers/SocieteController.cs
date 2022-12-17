using Projet.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Projet.Controllers
{
    public class SocieteController : Controller
    {
        private ApplicationDbContext _db = new ApplicationDbContext();

        public ActionResult Accueil()
        {
            if (Session["IDSociete"] != null)
            {
                return View();
            }
            return RedirectToAction("Inscription");

        }
        public ActionResult Inscription()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Inscription([Bind(Exclude ="IDSociete")] Societe societe)
        {
            if (ModelState.IsValid)
            {
                var check = _db.Societe.FirstOrDefault(s => s.Identifiant == societe.Identifiant);
                if (check == null)
                {
                    societe.MotDePasse = GetMD5(societe.MotDePasse);
                    _db.Configuration.ValidateOnSaveEnabled = false;
                    _db.Societe.Add(societe);
                    _db.SaveChanges();
                    Societe actuelle = _db.Societe.Find(societe.IDSociete);
                    Session["IDSociete"] = actuelle.IDSociete;
                    Session["Nom"] = actuelle.Nom;
                    Session["Identifiant"] = actuelle.Identifiant;
                    return RedirectToAction("Accueil");
                }
                else
                {
                    ModelState.AddModelError("Identifiant", "L'identifiant choisi est déja utilisé.");
                    return View();
                }


            }
            return View();


        }


        public ActionResult Connexion()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Connexion([Bind(Include = "Identifiant, MotDePasse")] Societe societe)
        {
            var password = GetMD5(societe.MotDePasse);
            var check = _db.Societe.SingleOrDefault(s => s.Identifiant.Equals(societe.Identifiant) && s.MotDePasse.Equals(password));

            if (check != null)
            {
                Session["IDSociete"] = check.IDSociete;
                Session["Nom"] = check.Nom;
                Session["Identifiant"] = check.Identifiant;
                return RedirectToAction("Accueil");

            }
            ModelState.AddModelError("MotDePasse", "L'identifiant ou le mot de passe sont incorrecte.");
            return View();

        }


        public ActionResult Offre()
        {
            if (Session["IDSociete"] != null) {
                ViewBag.IDAutocar = new SelectList(_db.Autocar, "IDAutocar", "Marque");
                ViewBag.IDVilleDep = new SelectList(_db.Ville, "IDVille", "Libelle");
                ViewBag.IDVilleDarr = new SelectList(_db.Ville, "IDVille", "Libelle");
                return View();
            }

            return RedirectToAction("Inscription");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Offre([Bind(Exclude = "IDSociete")] Abonnement abonnement, HttpPostedFileBase imageChoisie)
        {
            //Societe societeRecente = _db.Societe.Find(Session["IDSociete"]);

            if (ModelState.IsValid)
            {
                if (imageChoisie != null)
                {
                    string ext = Path.GetExtension(imageChoisie.FileName);
                    string NVNom = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss.fff") + ext;
                    imageChoisie.SaveAs(Server.MapPath("~/Images/" + NVNom));
                    abonnement.AutoImage = NVNom;
                }
                abonnement.nbrAtteint = 0;
                _db.Abonnement.Add(abonnement);
                _db.Societe.Find(Session["IDSociete"]).Abonnement.Add(abonnement);
                _db.SaveChanges();
                return RedirectToAction("Accueil");
            }
            ViewBag.IDAutocar = new SelectList(_db.Autocar.OrderBy(s => s.Marque), "IDAutocar", "Marque");
            ViewBag.IDVilleDep = new SelectList(_db.Ville.OrderBy(v => v.Libelle), "IDVille", "Libelle");
            ViewBag.IDVilleDarr = new SelectList(_db.Ville.OrderBy(v => v.Libelle), "IDVille", "Libelle");
            return View();
        }


        public ActionResult OffresCréés()
        {
            //var Result = ctx.Students.Where(st => st.StandardId == 1).Select(st => new { Id = st.StudentId, Name = st.StudentName }).ToList()
            //var Result = from s in ctx.Students where s.StandardId == 1 select new { Id = s.StudentId, Name = s.StudentName };
            if (Session["IDSociete"] != null) {
                int id = (int)Session["IDSociete"];
                var abonnement = _db.Abonnement.Where(a => a.IDSociete == id);
                return View(abonnement.ToList());
            }
            return RedirectToAction("Inscription");
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("OffresCréés");
            }
            Abonnement abonnement = _db.Abonnement.Find(id);
            if (abonnement == null)
            {
                return RedirectToAction("OffresCréés");
            }
            return View(abonnement);
        }

        public ActionResult Modifier(int id)
        {
            if (Session["IDSociete"] != null)
            {
                if (id == null)
                {
                    return RedirectToAction("OffresCréés");
                }
                Abonnement abonnement = _db.Abonnement.Find(id);
                if (abonnement == null)
                {
                    return RedirectToAction("OffresCréés");
                }
                ViewBag.IDAutocar = new SelectList(_db.Autocar.OrderBy(a=>a.Marque), "IDAutocar", "Marque", abonnement.IDAutocar);
                //ViewBag.IDSociete = new SelectList(_db.Societe, "IDSociete", "Nom", abonnement.IDSociete);
                ViewBag.IDVilleDep = new SelectList(_db.Ville.OrderBy(v => v.Libelle), "IDVille", "Libelle", abonnement.IDVilleDep);
                ViewBag.IDVilleDarr = new SelectList(_db.Ville.OrderBy(v => v.Libelle), "IDVille", "Libelle", abonnement.IDVilleDarr);
                return View(abonnement);
            }

            return RedirectToAction("Inscription");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Modifier(Abonnement abonnement, HttpPostedFileBase imageChoisie)
        {
            if (ModelState.IsValid)
            {
                if (imageChoisie != null)
                {
                    string ext = Path.GetExtension(imageChoisie.FileName);
                    string NVNom = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss.fff") + ext;
                    imageChoisie.SaveAs(Server.MapPath("~/Images/" + NVNom));
                    abonnement.AutoImage = NVNom;
                }
                _db.Entry(abonnement).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Accueil");
            }
            ViewBag.IDAutocar = new SelectList(_db.Autocar.OrderBy(a => a.Marque), "IDAutocar", "Marque", abonnement.IDAutocar);
            //ViewBag.IDSociete = new SelectList(_db.Societe, "IDSociete", "Nom", abonnement.IDSociete);
            ViewBag.IDVilleDep = new SelectList(_db.Ville.OrderBy(v => v.Libelle), "IDVille", "Libelle", abonnement.IDVilleDep);
            ViewBag.IDVilleDarr = new SelectList(_db.Ville.OrderBy(v => v.Libelle), "IDVille", "Libelle", abonnement.IDVilleDarr);
            return View(abonnement);
        }

        public ActionResult Supprimer(int? id)
        {
            if (Session["IDSociete"] != null)
            {
                if (id == null)
                {
                    return RedirectToAction("OffresCréés");
                }
                Abonnement abonnement = _db.Abonnement.Find(id);
                if (abonnement == null)
                {
                    return HttpNotFound();
                }
                return View(abonnement);
            }
            return RedirectToAction("Inscription");
        }

        // POST: Abonnements/Delete/5
        [HttpPost, ActionName("Supprimer")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Abonnement abonnement = _db.Abonnement.Find(id);
            _db.Abonnement.Remove(abonnement);
            var clients=_db.Client.ToList();
            foreach(var item in clients)
            {
                if (item.Abonnement.Contains(abonnement))
                {
                    item.Abonnement.Remove(abonnement);
                }
            }
            _db.SaveChanges();
            return RedirectToAction("OffresCréés");
        }


        public ActionResult Demandes()
        {
            //var Result = ctx.Students.Where(st => st.StandardId == 1).Select(st => new { Id = st.StudentId, Name = st.StudentName }).ToList()
            //var Result = from s in ctx.Students where s.StandardId == 1 select new { Id = s.StudentId, Name = s.StudentName };
            if (Session["IDSociete"] != null)
            {
                int id = (int)Session["IDSociete"];
                //var demande = _db.Demande.Where(d => d.IDSociete == id).OrderBy(d=>d.HeureDarr).ThenBy(d=>d.HeureDep).ThenBy(d => d.IDVilleDarr).ThenBy(d => d.IDVilleDep).ThenBy(d => d.DateDebut).ThenBy(d => d.DateFin);
                var demandes = _db.Demande.ToList();
                var demande = demandes.Where(n => n.IDSociete == id)
                    .GroupBy(n => new { DateDebut = n.DateDebut, DateFin = n.DateDebut, HeureDep = n.HeureDep, HeureDarr = n.HeureDarr, IDVilleDep = n.Ville.Libelle, IDVilleDarr = n.Ville1.Libelle, IDDemande=n.IDDemande })
                    .Select(g => new GérerDemande { ID=g.Key.IDDemande,DateDebut = g.Key.DateDebut, DateFin = g.Key.DateDebut, HeureDep = g.Key.HeureDep, HeureDarr = g.Key.HeureDarr, IDVilleDep = g.Key.IDVilleDep, IDVilleDarr = g.Key.IDVilleDarr,nbr=g.Count() });
                return View(demande.ToList());
            }
            return RedirectToAction("Inscription");
        }

        public ActionResult GérerDemande(int id)
        {
            if (Session["IDSociete"] != null)
            {
                if (id == null)
                {
                    return RedirectToAction("OffresCréés");
                }
                var demande = _db.Demande.Find(id);
                if (demande == null)
                {
                    return RedirectToAction("OffresCréés");
                }
                Abonnement abonnement = new Abonnement
                {
                    DateDebut = demande.DateDebut,
                    DateFin = demande.DateFin,
                    IDVilleDep = demande.IDVilleDep,
                    IDVilleDarr = demande.IDVilleDarr,
                    HeureDep=demande.HeureDep,
                    HeureDarr=demande.HeureDarr,
                };
                ViewBag.IDAutocar = new SelectList(_db.Autocar.OrderBy(a => a.Marque), "IDAutocar", "Marque", abonnement.IDAutocar);
                ViewBag.IDVilleDep = new SelectList(_db.Ville.OrderBy(v => v.Libelle), "IDVille", "Libelle", abonnement.IDVilleDep);
                ViewBag.IDVilleDarr = new SelectList(_db.Ville.OrderBy(v => v.Libelle), "IDVille", "Libelle", abonnement.IDVilleDarr);
                return View(abonnement);
            }
           
            return RedirectToAction("Inscription");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult GérerDemande([Bind(Exclude = "IDSociete")] Abonnement abonnement, HttpPostedFileBase imageChoisie)
        {
            if (ModelState.IsValid)
            {
                if (imageChoisie != null)
                {
                    string ext = Path.GetExtension(imageChoisie.FileName);
                    string NVNom = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss.fff") + ext;
                    imageChoisie.SaveAs(Server.MapPath("~/Images/" + NVNom));
                    abonnement.AutoImage = NVNom;
                }
                abonnement.nbrAtteint = 0;
                _db.Abonnement.Add(abonnement);
                //Abonnement abonnementCréé =_db.Abonnement.Find(abonnement.IDAbonnement);
                //abonnementCréé.Societe = societeRecente;
                _db.Societe.Find(Session["IDSociete"]).Abonnement.Add(abonnement);
                _db.SaveChanges();
                return RedirectToAction("Accueil");
            }
            ViewBag.IDAutocar = new SelectList(_db.Autocar.OrderBy(s => s.Marque), "IDAutocar", "Marque");
            ViewBag.IDVilleDep = new SelectList(_db.Ville.OrderBy(v => v.Libelle), "IDVille", "Libelle");
            ViewBag.IDVilleDarr = new SelectList(_db.Ville.OrderBy(v => v.Libelle), "IDVille", "Libelle");
            return View();
        }

        public ActionResult Données()
        {
            if (Session["IDSociete"] != null) {
                int id = (int)Session["IDSociete"];
                Societe societe = _db.Societe.Find(id);
                if (societe == null)
                {
                    return HttpNotFound();
                }
                return View(societe);
            }
            return RedirectToAction("Inscription");
            
        }

        public ActionResult ModifierDonnées() {
            if (Session["IDSociete"] != null)
            {
                int id = (int)Session["IDSociete"];
                Societe societe =_db.Societe.Find(id);
                return View(societe);
            }
            return RedirectToAction("Inscription");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ModifierDonnées(Societe societe)
        {
            if (ModelState.IsValid)
            {
                societe.MotDePasse = GetMD5(societe.MotDePasse);
                _db.Entry(societe).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Accueil");
            }
            return View();
        }

        //Logout
        public ActionResult Déconnexion()
        {
            Session.Clear();//remove session
            return RedirectToAction("Index","Home");
        }



        //create a string MD5
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
        }

    }
}