using Projet.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Projet.Controllers
{
    public class ClientController : Controller
    {
        // GET: Client
        private ApplicationDbContext _db = new ApplicationDbContext();

        public ActionResult Accueil()
        {
            if (Session["IDClient"] != null)
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
        public ActionResult Inscription(Client client)
        {
            if (ModelState.IsValid)
            {
                var check = _db.Client.FirstOrDefault(c=> c.Email == client.Email);
                if (check == null)
                {
                    client.MotDePasse = GetMD5(client.MotDePasse);
                    _db.Configuration.ValidateOnSaveEnabled = false;
                    _db.Client.Add(client);
                    _db.SaveChanges();
                    Client actuel = _db.Client.Find(client.IDClient);
                    Session["IDClient"] = actuel.IDClient;
                    Session["NomComplet"] = actuel.Nom + " " +actuel.prenom;
                    Session["Email"] = actuel.Email;
                    return RedirectToAction("Accueil");
                }
                else
                {
                    ModelState.AddModelError("Email", "L'email choisi est déja utilisé.");
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
        public ActionResult Connexion([Bind(Include ="Email,MotDePasse")]Client client)
        {
            var password = GetMD5(client.MotDePasse);
            var check = _db.Client.SingleOrDefault(s => s.Email.Equals(client.Email) && s.MotDePasse.Equals(password));

            if (check != null)
            {
                Session["IDClient"] = check.IDClient;
                Session["NomComplet"] = check.Nom + " " + check.prenom;
                Session["Email"] = check.Email;
                return RedirectToAction("Accueil");

            }
            ModelState.AddModelError("MotDePasse", "L'identifiant ou le mot de passe sont incorrectes.");
            return View();

        }


        public ActionResult Abonnement()
        {
            if (Session["IDClient"] != null)
            {
                var abonnements=_db.Abonnement.ToList();
                return View(abonnements);
            }

            return RedirectToAction("Inscription");
        }


        
        public ActionResult SAbonner(int id)
        {
            //Societe societeRecente = _db.Societe.Find(Session["IDSociete"]);
            if (Session["IDClient"] != null) {
                Boolean verify = true;
                int idClient = (int)Session["IDClient"];
                Abonnement abonnement =_db.Abonnement.Find(id);
                Client client = _db.Client.Find(idClient);
                //Abonnement abonnementCréé =_db.Abonnement.Find(abonnement.IDAbonnement);
                //abonnementCréé.Societe = societeRecente;
                var clientsAbonn = _db.Abonnement.Find(abonnement.IDAbonnement).Client.ToList();
                foreach (var clt in clientsAbonn) {
                    if (_db.Abonnement.Find(abonnement.IDAbonnement).Client.Contains(client) != false) {
                        verify = false;
                    }
                }
                if (verify)
                {
                    _db.Client.Find(idClient).Abonnement.Add(abonnement);
                    _db.Abonnement.Find(id).Client.Add(client);
                    _db.Abonnement.Find(id).nbrAtteint++;
                    _db.SaveChanges();
                    return RedirectToAction("Accueil");
                }
            }
            return RedirectToAction("Abonnement");
        }


        public ActionResult MesAbonnement()
        {
            //var Result = ctx.Students.Where(st => st.StandardId == 1).Select(st => new { Id = st.StudentId, Name = st.StudentName }).ToList()
            //var Result = from s in ctx.Students where s.StandardId == 1 select new { Id = s.StudentId, Name = s.StudentName };
            if (Session["IDClient"] != null)
            {
                int id = (int)Session["IDClient"];
                var abonnements = _db.Client.Find(id).Abonnement;
                //var abonnements=_db.Abonnement.Find()
                return View(abonnements.ToList());
            }
            return RedirectToAction("Inscription");
        }

        public ActionResult Désabonner(int id)
        {
            if (Session["IDClient"] != null)
            {
                Abonnement abonnement = _db.Abonnement.Find(id);
                return View(abonnement);
            }
            return RedirectToAction("Inscription");
        }

        [HttpPost, ActionName("Désabonner")]
        [ValidateAntiForgeryToken]
        public ActionResult DésabonnerConfirmed(int id)
        {
            Abonnement abonnement = _db.Abonnement.Find(id);
            _db.Client.Find(Session["IDClient"]).Abonnement.Remove(abonnement);
            _db.Abonnement.Find(abonnement.IDAbonnement).nbrAtteint--;
            _db.SaveChanges();
            return RedirectToAction("Accueil");
        }


        public ActionResult Demande()
        {
            if (Session["IDClient"] != null)
            {
                ViewBag.IDSociete = new SelectList(_db.Societe.OrderBy(s=>s.Nom), "IDSociete", "Nom");
                ViewBag.IDVilleDep = new SelectList(_db.Ville.OrderBy(v=>v.Libelle), "IDVille", "Libelle");
                ViewBag.IDVilleDarr = new SelectList(_db.Ville.OrderBy(v => v.Libelle), "IDVille", "Libelle");
                return View();
            }
            return RedirectToAction("Inscription");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Demande(Demande demande)
        { 
            if (ModelState.IsValid)
            {
                int id = (int)Session["IDClient"];
                _db.Demande.Add(demande);
                int idDemande = _db.Demande.Find(demande.IDDemande).IDDemande;
                Client clientActuel = _db.Client.Find(id);
                _db.Client.Find(id).Demande.Add(demande);
                _db.Demande.Find(idDemande).Client.Add(clientActuel);
                _db.SaveChanges();
                return RedirectToAction("Accueil");
            }
            ViewBag.IDSociete = new SelectList(_db.Societe.OrderBy(s=>s.Nom), "IDSociete", "Nom", demande.IDSociete);
            ViewBag.IDVilleDep = new SelectList(_db.Ville.OrderBy(v => v.Libelle), "IDVille", "Libelle", demande.IDVilleDep);
            ViewBag.IDVilleDarr = new SelectList(_db.Ville.OrderBy(v => v.Libelle), "IDVille", "Libelle", demande.IDVilleDarr);
            return View();
        }


        public ActionResult Données()
        {
            int id = (int)Session["IDClient"];
            Client client = _db.Client.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }
        public ActionResult ModifierDonnées(int id)
        {
            if (Session["IDClient"] != null)
            {
                Client client = _db.Client.Find(id);
                return View(client);
            }
            return RedirectToAction("Inscription");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ModifierDonnées([Bind(Exclude = "IDClient")] Client client)
        {
            if (ModelState.IsValid)
            {
                client.MotDePasse = GetMD5(client.MotDePasse);
                _db.Entry(client).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Accueil");
            }
            return RedirectToAction("Inscription");
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