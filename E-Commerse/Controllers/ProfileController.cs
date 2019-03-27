using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using E_Commerse.Models;
using E_Commerse.ViewModels.Home;
using E_Commerse.Models.Managers;

namespace E_Commerse.Controllers
{
    public class ProfileController : Controller
    {
        // GET: Profile
        public ActionResult ProfilePage(int? urunid)
        {
            DatabaseContext db = new DatabaseContext();
            HomePageModel model = new HomePageModel();
            int id = Convert.ToInt32(Session["kullaniciID"]);
            if (urunid!=null)
            {
                Favorites fav = db.favorites.Where(X => X.urunID.urunID == urunid).FirstOrDefault();
                db.favorites.Remove(fav);
                db.SaveChanges();
            }

                var favoriurun = (from m in db.favorites
                                  where m.kullaniciID.kullaniciID == id
                                  select m).ToList();
                model.favorites = favoriurun;

            
            
            model.urunler = db.urunler.ToList();          //select * from urunler
            model.kategoriler = db.kategoriler.ToList();  //select * from kategoriler
            return View(model);
        }
        // POST: Profile
        [HttpPost]
        public ActionResult ProfilePage(string btnLogout)
        {
            DatabaseContext db = new DatabaseContext();
            HomePageModel model = new HomePageModel();
            if (btnLogout != null)
            {
                Session["loginDurum"] = 0;
                model.urunler = db.urunler.ToList();          //select * from urunler
                return RedirectToAction("Index", "Home");

            }
            model.kategoriler = db.kategoriler.ToList(); //select * from kategoriler
            model.favorites = db.favorites.ToList();  
            return View(model);
        }


    }
}