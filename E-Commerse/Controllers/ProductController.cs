using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using E_Commerse.Models;
using E_Commerse.ViewModels.Home;
using E_Commerse.Models.Managers;

namespace E_Commerse.Controllers
{
    public class ProductController : Controller
    {
        // GET: ProductGrid
        public ActionResult ProductGridKategori(int? kategoriID,string keyword)
        {
            DatabaseContext db = new DatabaseContext();
            HomePageModel model = new HomePageModel();
            if (keyword != null)
            {
                var aramaUrun = (from m in db.urunler
                                 where m.urunAd.Contains(keyword)
                                 select m).ToList();
                model.urunler = aramaUrun;
                model.kategoriler = db.kategoriler.ToList();

            }
            else
            {
                var kategoriUrun = (from m in db.urunler
                                    where m.kategoriID.kategoriID == kategoriID
                                    select m).ToList();
                model.urunler = kategoriUrun;
                model.kategoriler = db.kategoriler.ToList();
                TempData["kategoriID"] = kategoriID;
            }                  
            
            return View("ProductGrid", model);
        }
        // GET: ProductGrid
        public ActionResult ProductGrid()
        {
            DatabaseContext db = new DatabaseContext();
            HomePageModel model = new HomePageModel();
            ViewData["kategoriID"] = TempData["kategoriID"];
            int id = Convert.ToInt32(ViewData["kategoriID"]);
            model.urunler = db.urunler.Where(x=>x.kategoriID.kategoriID== id).ToList();
            model.kategoriler = db.kategoriler.ToList();
            
             
            return View(model);
        }
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
            }

            model.urunler = db.urunler.ToList();          //select * from urunler
            model.kategoriler = db.kategoriler.ToList();  //select * from kategoriler
            Response.Redirect("Index");
            return View(model);
        }


        [HttpPost]
        public ActionResult ProductGridKategori(string txtKeyword,decimal? minPrice, decimal? maxPrice, int? kategoriID,string eposta,string sifre,string btnLogin,string btnLogout,string btnSearch,string btnPriceRange)
        {
            DatabaseContext db = new DatabaseContext();
            HomePageModel model = new HomePageModel();
            if (btnLogin != null)
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
                //select * from kategoriler
                TempData["kategoriID"] = kategoriID;
                ViewData["kategoriID"] = kategoriID;
                Response.Redirect(Request.RawUrl);
            }
            else if (btnLogout != null)
            {
                Session["loginDurum"] = 0;
                model.urunler = db.urunler.ToList();          //select * from urunler
                Response.Redirect(Request.RawUrl);
                Response.Clear();
                Response.BufferOutput = true;
            }
            else if (btnPriceRange!=null)
            {
                if (minPrice < maxPrice)
                {
                    var kategoriUrun = (from m in db.urunler
                                        where m.kategoriID.kategoriID == kategoriID
                                        && (m.urunFiyat - (m.urunFiyat * m.indirimYuzde / 100)) >= minPrice && (m.urunFiyat - (m.urunFiyat * m.indirimYuzde / 100)) <= maxPrice
                                        select m).ToList();
                    model.urunler = kategoriUrun;
                   
                }
            }
           else if (btnSearch!=null)
            {
                var aramaUrun = (from m in db.urunler
                                 where m.urunAd.Contains(txtKeyword)
                                 select m).ToList();
                model.urunler = aramaUrun;
               
            }

            model.kategoriler = db.kategoriler.ToList();
            TempData["kategoriID"] = kategoriID;
            ViewData["kategoriID"] = kategoriID;
            return View("ProductGrid", model);
            
        }
    

        // GET: ProductDetails
        public ActionResult ProductDetails(int? urunid)
        {
            ViewData["hide"] = "block";
            DatabaseContext db = new DatabaseContext();
            HomePageModel model = new HomePageModel();
            model.kategoriler = db.kategoriler.ToList();
            model.seciliUrun = null;
            model.sepet = db.sepet.ToList();

            if (urunid != null)
            {
                var kategoriUrun = (from m in db.urunler
                                    where m.urunID == urunid
                                    select m).First();
                model.seciliUrun = kategoriUrun;
            }

            return View(model);
        }
        [HttpPost]
        public ActionResult ProductDetails(int? urunid,string text,string addCart)
        {
            DatabaseContext db = new DatabaseContext();
            HomePageModel model = new HomePageModel();
            var kullaniciID = Convert.ToInt32(Session["kullaniciID"]);
            model.sepet = db.sepet.ToList();
            if (addCart!=null)
            {
                Session["sepetAdet"] = Convert.ToInt32( Session["sepetAdet"] )+ 1;
                Sepet sepet = new Sepet();
                sepet.urunID = (from m in db.urunler
                                where m.urunID == urunid
                                select m).FirstOrDefault();
                sepet.kullaniciID= (from m in db.kullanici
                                    where m.kullaniciID == kullaniciID
                                    select m).FirstOrDefault();
                sepet.adet = 1;
                
                try
                {
                    db.sepet.Add(sepet);
                    db.SaveChanges();
                }
                catch (Exception)
                {

                    throw;
                }
                
            }
            
            ViewData["hide"] = "none";
        
            model.kategoriler = db.kategoriler.ToList();
            model.seciliUrun = null;

                var kategoriUrun = (from m in db.urunler
                                    where m.urunID == urunid
                                    select m).First();
                model.seciliUrun = kategoriUrun;



            Favorites f = new Favorites();
            var sayi = 0;
            sayi = (int)Session["kullaniciID"];
            f.urunID = kategoriUrun;
            var kullanici = (from m in db.kullanici
                             where m.kullaniciID == sayi
                             select m).FirstOrDefault();
            f.kullaniciID = kullanici;




            db.favorites.Add(f);
            db.SaveChanges();


            model.kategoriler = db.kategoriler.ToList();
            return View("ProductDetails",model);
        }

    }
}