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
    public class HomeController : Controller
    {
        // GET: Register
        public ActionResult Register()
        {
            DatabaseContext db = new DatabaseContext();
            HomePageModel model = new HomePageModel();
            if (TempData["errorRegister"]!=null)
            {

                ViewData["errorRegister"] = TempData["errorRegister"];

            }
            if (TempData["errorRegisterEmpty"] != null)
            {

                ViewData["errorRegisterEmpty"] = TempData["errorRegisterEmpty"];

            }

            model.kategoriler = db.kategoriler.ToList();  //select * from kategoriler
            return View(model);
        }
        // POST: Register
        [HttpPost]
        public ActionResult Register(string eposta, string sifre, string btnLogin, string firstname, string lastname, string email, string password, string address, string country, string btnRegister, int? isRegisterOk)
        {
            DatabaseContext db = new DatabaseContext();
            HomePageModel model = new HomePageModel();
            if (firstname==""&&lastname==""&& email == "" && password == ""&& address == "" && country == "")
            {
                TempData["errorRegisterEmpty"] = 1;
                return RedirectToAction("Register");
            }
            if (btnRegister != null)
            {
                Kullanici isSame = null;
                isSame = db.kullanici.Where(x => x.email == email).FirstOrDefault();
                if (isSame == null)
                {
                    Kullanici kullanici = new Kullanici
                    {
                        kullaniciAd = firstname,
                        kullaniciSoyAd = lastname,
                        email = email,
                        kullaniciSifre = password,
                        Adres = address,
                        Country = country,
                        memberSince = DateTime.Now,
                    };
                    db.kullanici.Add(kullanici);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["errorRegister"] = 1;
                    return RedirectToAction("Register");
                }
            }
          
            else if (btnLogin != null)
            {
                try
                {
                    var kullanici = (from m in db.kullanici
                                     where m.email == eposta && m.kullaniciSifre == sifre
                                     select m).FirstOrDefault();

                    Session["kullaniciID"] = kullanici.kullaniciID;
                    Session["kullaniciAd"] = kullanici.kullaniciAd;
                    Session["kullaniciSoyAd"] = kullanici.kullaniciSoyAd;
                    Session["kullaniciemail"] = kullanici.email;
                    Session["kullaniciSifre"] = kullanici.kullaniciSifre;
                    Session["kullaniciAdres"] = kullanici.Adres;
                    Session["kullaniciUlke"] = kullanici.Country;
                    Session["kullaniciUyelikTarihi"] = kullanici.memberSince;
                    Session["loginDurum"] = 1;
                    model.urunler = db.urunler.ToList();          //select * from urunler
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception)
                {
                    //KULLANICI BULUNAMADI.
                    model.urunler = db.urunler.ToList();
                    ViewData["alertLogin"] = "1";

                }

            }
            model.kategoriler = db.kategoriler.ToList();  
            return View(model);
        }
        // GET: Home
        public ActionResult Index()
        {
            DatabaseContext db = new DatabaseContext();
            HomePageModel model = new HomePageModel();
            model.urunler = db.urunler.ToList();          //select * from urunler
            model.kategoriler = db.kategoriler.ToList();  //select * from kategoriler
            return View(model);
        }
        [HttpPost]
        public ActionResult Index(string eposta,string sifre, string btnLogin, string btnLogout)
        {
            DatabaseContext db = new DatabaseContext();
            HomePageModel model = new HomePageModel();
            
            if (btnLogin != null)
            {
                try
                {
                    var kullanici = (from m in db.kullanici
                                     where m.email == eposta && m.kullaniciSifre == sifre
                                     select m).FirstOrDefault();

                    Session["kullaniciID"] = kullanici.kullaniciID;
                    Session["kullaniciAd"] = kullanici.kullaniciAd;
                    Session["kullaniciSoyAd"] = kullanici.kullaniciSoyAd;
                    Session["kullaniciemail"] = kullanici.email;
                    Session["kullaniciSifre"] = kullanici.kullaniciSifre;
                    Session["kullaniciAdres"] = kullanici.Adres;
                    Session["kullaniciUlke"] = kullanici.Country;
                    Session["kullaniciUyelikTarihi"] = kullanici.memberSince;
                    Session["loginDurum"] = 1;
                    Session["sepetAdet"] = 0;
                    model.urunler = db.urunler.ToList();          //select * from urunler
                    Response.Redirect(Request.RawUrl);
                }
                catch (Exception)
                {
                    //TODO BU KULLANICI BULUNAMADI.
                    model.urunler = db.urunler.ToList();
                    ViewData["alertLogin"] = "1";
                    
                }
                
            }
            else if (btnLogout != null)
            {
                Session["loginDurum"] = 0;
                model.urunler = db.urunler.ToList();          //select * from urunler
                Response.Redirect(Request.RawUrl);
                Response.Clear();
                Response.BufferOutput = true;
            }
            model.kategoriler = db.kategoriler.ToList();
            return View(model);
        }
        //GET: Contact
        public ActionResult Contact()
        {
            DatabaseContext db = new DatabaseContext();
            HomePageModel model = new HomePageModel();
            model.urunler = db.urunler.ToList();          //select * from urunler
            model.kategoriler = db.kategoriler.ToList();  //select * from kategoriler
            return View(model);
        }
        [HttpPost]
        public ActionResult Contact(string eposta, string sifre)
        {
            
            DatabaseContext db = new DatabaseContext();
            HomePageModel model = new HomePageModel();
            var kullanici = (from m in db.kullanici
                             where m.email == eposta && m.kullaniciSifre == sifre
                             select m).FirstOrDefault();
            if (kullanici != null)
            {
                Session["kullaniciID"] = kullanici.kullaniciID;
                Session["kullaniciAd"] = kullanici.kullaniciAd;
                Session["kullaniciSoyAd"] = kullanici.kullaniciSoyAd;
                Session["kullaniciemail"] = kullanici.email;
                Session["kullaniciSifre"] = kullanici.kullaniciSifre;
                Session["kullaniciAdres"] = kullanici.Adres;
                Session["kullaniciUlke"] = kullanici.Country;
                Session["kullaniciUyelikTarihi"] = kullanici.memberSince;
                Session["loginDurum"] = 1;
                model.urunler = db.urunler.ToList();          //select * from urunler
                model.kategoriler = db.kategoriler.ToList();  //select * from kategoriler
                return View("Contact",model);
            }
            else
            {
                Session["loginDurum"] = 0;
                model.urunler = db.urunler.ToList();          //select * from urunler
                model.kategoriler = db.kategoriler.ToList();  //select * from kategoriler
                Response.Redirect("Index");
                Response.Clear();
                Response.BufferOutput = true;
            }
            
           
            return View(model);
        }

        //GET: About
        public ActionResult About()
        {
            DatabaseContext db = new DatabaseContext();
            HomePageModel model = new HomePageModel();
            model.urunler = db.urunler.ToList();          //select * from urunler
            model.kategoriler = db.kategoriler.ToList();  //select * from kategoriler
            return View(model);
        }
        //POST:About
        [HttpPost]
        public ActionResult About(string eposta, string sifre)
        {

            DatabaseContext db = new DatabaseContext();
            HomePageModel model = new HomePageModel();
            var kullanici = (from m in db.kullanici
                             where m.email == eposta && m.kullaniciSifre == sifre
                             select m).FirstOrDefault();
            if (kullanici != null)
            {
                Session["kullaniciID"] = kullanici.kullaniciID;
                Session["kullaniciAd"] = kullanici.kullaniciAd;
                Session["kullaniciSoyAd"] = kullanici.kullaniciSoyAd;
                Session["kullaniciEmail"] = kullanici.email;
                Session["kullaniciSifre"] = kullanici.kullaniciSifre;
                Session["kullaniciAdres"] = kullanici.Adres;
                Session["kullaniciUlke"] = kullanici.Country;
                Session["kullaniciUyelikTarihi"] = kullanici.memberSince;
                Session["loginDurum"] = 1;
                model.urunler = db.urunler.ToList();          //select * from urunler
                model.kategoriler = db.kategoriler.ToList();  //select * from kategoriler
                return View("About", model);
            }
            else
            {
                Session["loginDurum"] = 0;
                model.urunler = db.urunler.ToList();          //select * from urunler
                model.kategoriler = db.kategoriler.ToList();  //select * from kategoriler
                Response.Redirect("Index");
                Response.Clear();
                Response.BufferOutput = true;
                
            }

            return View("Index", model);

        }
        //GET: Help
        public ActionResult Help()
        {
            DatabaseContext db = new DatabaseContext();
            HomePageModel model = new HomePageModel();
            model.urunler = db.urunler.ToList();          //select * from urunler
            model.kategoriler = db.kategoriler.ToList();  //select * from kategoriler
            return View(model);
        }
        //POST:Help
        [HttpPost]
        public ActionResult Help(string eposta, string sifre)
        {


            DatabaseContext db = new DatabaseContext();
            HomePageModel model = new HomePageModel();
            var kullanici = (from m in db.kullanici
                             where m.email == eposta && m.kullaniciSifre == sifre
                             select m).FirstOrDefault();
            if (kullanici != null)
            {
                Session["kullaniciID"] = kullanici.kullaniciID;
                Session["kullaniciAd"] = kullanici.kullaniciAd;
                Session["kullaniciSoyAd"] = kullanici.kullaniciSoyAd;
                Session["kullaniciEmail"] = kullanici.email;
                Session["kullaniciAdres"] = kullanici.Adres;
                Session["kullaniciUlke"] = kullanici.Country;
                Session["kullaniciUyelikTarihi"] = kullanici.memberSince;
                Session["loginDurum"] = 1;
                model.urunler = db.urunler.ToList();          //select * from urunler
                model.kategoriler = db.kategoriler.ToList();  //select * from kategoriler
                return View("Help", model);

            }
            else
            {
                Session["loginDurum"] = 0;
                model.urunler = db.urunler.ToList();          //select * from urunler
                model.kategoriler = db.kategoriler.ToList();  //select * from kategoriler
                Response.Redirect("Index");
                Response.Clear();
                Response.BufferOutput = true;
            }


            return View(model);
        }
    }
}