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

            if (Convert.ToInt32(Session["kategoriID"]) > 0)
            {
                kategoriID = Convert.ToInt32(Session["kategoriID"]);
            }
            DatabaseContext db = new DatabaseContext();
            HomePageModel model = new HomePageModel();
            if (kategoriID==null)
            {
                return RedirectToAction("ErrorPage", "Home");
            }
            var kategoriNow = db.kategoriler.Where(x => x.kategoriID == kategoriID).FirstOrDefault();

            Session["KategoriAd"] = kategoriNow.kategoriAd;
            var kullaniciID = Convert.ToInt32(Session["kullaniciID"]);
            if (Convert.ToInt32(Session["loginDurum"]) != 1)
            {
                model.sepet = db.sepet.Where(x => x.kullaniciID == null).ToList();
                Session["sepetAdetZiyaretci"] = (from m in db.sepet
                                                 where m.kullaniciID == null
                                                 select m.urunAdet).DefaultIfEmpty(0).Sum();
            }
            else
            {
                model.sepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
                Session["sepetAdetKullanici"] = (from m in db.sepet
                                                 where m.kullaniciID.kullaniciID == kullaniciID
                                                 select m.urunAdet).DefaultIfEmpty(0).Sum();
            }
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
                int id = Convert.ToInt32(Session["kategoriID"]);
                TempData["kategoriID"] = id;
            }                  
            
            return View("ProductGrid", model);
        }
        // GET: ProductGrid
        
        
        [HttpPost]
        public ActionResult ProductGridKategori(string btnSil,string txtKeyword,decimal? minPrice, decimal? maxPrice, int? kategoriID,string eposta,string sifre,string btnLogin,string btnLogout,string btnSearch,string btnPriceRange)
        {
            DatabaseContext db = new DatabaseContext();
            HomePageModel model = new HomePageModel();
            var kullaniciID = Convert.ToInt32(Session["kullaniciID"]);
            var kategoriUrun = (from m in db.urunler
                                where m.kategoriID.kategoriID == kategoriID
                                select m).ToList();
            if (btnLogin != null)
            {
                var kullanici = (from m in db.kullanici
                                 where m.email == eposta && m.kullaniciSifre == sifre
                                 select m).FirstOrDefault();
                try
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
                    Session["sepetAdet"] = 0;
                    kullaniciID = Convert.ToInt32(Session["kullaniciID"]);
                    model.urunler = db.urunler.ToList();          //select * from urunler
                }
                catch (Exception)
                {
                    //TODO BU KULLANICI BULUNAMADI.
                    model.urunler = db.urunler.ToList();   
                    ViewData["alertLogin"] = "1";
                    Session["kategoriID"] = kategoriID;
                    var fullUrl = this.Url.Action("ProductGridKategori", "Product", new { kategoriID = kategoriID });

                    return Redirect(fullUrl);

                }
                kullaniciID = Convert.ToInt32(Session["kullaniciID"]);
                var ziyaretciSepetList = (from m in db.sepet
                                          where m.kullaniciID == null
                                          select m).ToList();
                foreach (var item in ziyaretciSepetList)
                {
                    int adet = (from m in db.sepet
                                where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == item.urunID.urunID
                                select m.urunAdet).DefaultIfEmpty(0).Sum();
                    if (adet == 0)
                    {
                        item.kullaniciID = (from m in db.kullanici
                                            where m.kullaniciID == kullaniciID
                                            select m).FirstOrDefault();
                        db.sepet.Add(item);
                        db.SaveChanges();

                    }
                    else
                    {
                        var sepetGüncelle = (from m in db.sepet
                                             where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == item.urunID.urunID
                                             select m).FirstOrDefault();

                        sepetGüncelle.urunAdet += item.urunAdet;
                        db.SaveChanges();
                    }

                    model.sepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
                    //last

                }
                model.urunler = db.urunler.ToList();          //select * from urunler
                model.sepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
                Response.Redirect(Request.RawUrl);
            }
            else if (btnLogout != null)
            {
                Session["loginDurum"] = 0;
                model.urunler = db.urunler.ToList();
                model.sepet = db.sepet.Where(x => x.kullaniciID == null).ToList();
                Response.Redirect(Request.RawUrl);

            }

            else if (btnSil != null)
            {

                model.kategoriler = db.kategoriler.ToList();
                TempData["kategoriID"] = kategoriID;
                ViewData["kategoriID"] = kategoriID;
                model.urunler = kategoriUrun;
                if (Convert.ToInt32(Session["loginDurum"]) == 1)
                {
                    int btnSilint = Convert.ToInt32(btnSil);
                    int adet = (from m in db.sepet
                                where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == btnSilint
                                select m.urunAdet).DefaultIfEmpty(0).Sum();
                    if (adet > 1)
                    {
                        var sepetGüncelle = (from m in db.sepet
                                             where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == btnSilint
                                             select m).FirstOrDefault();

                        sepetGüncelle.urunAdet--;
                        db.SaveChanges();
                    }
                    else
                    {
                        Sepet sepetvol1 = (from m in db.sepet
                                           where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == btnSilint
                                           select m).FirstOrDefault();
                        db.sepet.Remove(sepetvol1);
                        db.SaveChanges();
                    }

                    model.sepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
                    Response.Redirect(Request.RawUrl);
                }
                //ZİYATETÇİ
                else
                {
                    int btnSilint = Convert.ToInt32(btnSil);
                    int adet = (from m in db.sepet
                                where m.kullaniciID == null && m.urunID.urunID == btnSilint
                                select m.urunAdet).DefaultIfEmpty(0).Sum();
                    if (adet > 1)
                    {
                        var sepetGüncelle = (from m in db.sepet
                                             where m.kullaniciID == null && m.urunID.urunID == btnSilint
                                             select m).FirstOrDefault();

                        sepetGüncelle.urunAdet--;
                        db.SaveChanges();
                    }
                    else
                    {
                        Sepet sepetvol1 = (from m in db.sepet
                                           where m.kullaniciID == null && m.urunID.urunID == btnSilint
                                           select m).FirstOrDefault();
                        db.sepet.Remove(sepetvol1);
                        db.SaveChanges();
                    }

                    model.sepet = db.sepet.Where(x => x.kullaniciID == null).ToList();
                    Response.Redirect(Request.RawUrl);
                }


            }
            else if (btnPriceRange!=null)
            {
                if (Convert.ToInt32(Session["loginDurum"])==1)
                {
                    model.sepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
                }
                else
                {
                    model.sepet = db.sepet.Where(x => x.kullaniciID == null).ToList();
                }
                if (minPrice < maxPrice)
                {
                    var kategoriUrunFiyat = (from m in db.urunler
                                        where m.kategoriID.kategoriID == kategoriID
                                        && (m.urunFiyat - (m.urunFiyat * m.indirimYuzde / 100)) >= minPrice && (m.urunFiyat - (m.urunFiyat * m.indirimYuzde / 100)) <= maxPrice
                                        select m).ToList();
                    model.urunler = kategoriUrunFiyat;
                   
                }


            }
           else if (btnSearch!=null)
            {
                var aramaUrun = (from m in db.urunler
                                 where m.urunAd.Contains(txtKeyword)
                                 select m).ToList();
                model.urunler = aramaUrun;
                if (Convert.ToInt32(Session["loginDurum"])==1)
                {
                    model.sepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();

                }
                else
                {
                    model.sepet = db.sepet.Where(x => x.kullaniciID == null).ToList();
                }

            }
            model.kategoriler = db.kategoriler.ToList();
            
            return View("ProductGrid", model);
            
        }

    

        // GET: ProductDetails
        public ActionResult ProductDetails(int? urunid)
        {
            ViewData["hide"] = "block";
            var kullaniciID = Convert.ToInt32(Session["kullaniciID"]);
            DatabaseContext db = new DatabaseContext();
            HomePageModel model = new HomePageModel();

            model.kategoriler = db.kategoriler.ToList();
            model.seciliUrun = null;
            model.seciliUrun = db.urunler.Where(x => x.urunID == urunid).FirstOrDefault();
            if (Convert.ToInt32(Session["loginDurum"]) != 1)
            {
                model.sepet = db.sepet.Where(x => x.kullaniciID == null).ToList();
                Session["sepetAdetZiyaretci"] = (from m in db.sepet
                                                 where m.kullaniciID == null
                                                 select m.urunAdet).DefaultIfEmpty(0).Sum();
            }
            else
            {
                model.sepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
                Session["sepetAdetKullanici"] = (from m in db.sepet
                                                 where m.kullaniciID.kullaniciID == kullaniciID
                                                 select m.urunAdet).DefaultIfEmpty(0).Sum();
            }
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
        public ActionResult ProductDetails(int? urunid,string text,string addCart,string addFavorites,string btnSil,string btnLogin,string btnLogout,string eposta,string sifre)
        {
            DatabaseContext db = new DatabaseContext();
            HomePageModel model = new HomePageModel();
            var kullaniciID = Convert.ToInt32(Session["kullaniciID"]);
            var kategoriUrun = (from m in db.urunler
                                where m.urunID == urunid
                                select m).First();

            if (btnLogin != null)
            {
                var kullanici = (from m in db.kullanici
                                 where m.email == eposta && m.kullaniciSifre == sifre
                                 select m).FirstOrDefault();
                var ziyaretciSepetList = (from m in db.sepet
                                          where m.kullaniciID == null
                                          select m).ToList();
                try
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
                    Session["sepetAdet"] = 0;
                    kullaniciID = Convert.ToInt32(Session["kullaniciID"]);
                    model.urunler = db.urunler.ToList();          //select * from urunler
                }
                catch (Exception)
                {
                    //TODO BU KULLANICI BULUNAMADI.
                    model.urunler = db.urunler.ToList();
                    ViewData["alertLogin"] = "1";
                    var fullUrl = this.Url.Action("ProductDetails", "Product", new { urunid = urunid });
                   
                    return Redirect(fullUrl);

                }
                foreach (var item in ziyaretciSepetList)
                {
                    int adet = (from m in db.sepet
                                where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == item.urunID.urunID
                                select m.urunAdet).DefaultIfEmpty(0).Sum();
                    if (adet == 0)
                    {
                        item.kullaniciID = (from m in db.kullanici
                                            where m.kullaniciID == kullaniciID
                                            select m).FirstOrDefault();
                        db.sepet.Add(item);
                        db.SaveChanges();

                    }
                    else
                    {
                        var sepetGüncelle = (from m in db.sepet
                                             where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == item.urunID.urunID
                                             select m).FirstOrDefault();

                        sepetGüncelle.urunAdet += item.urunAdet;
                        db.SaveChanges();
                    }

                    model.sepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
                    //last
                    
                }
                model.urunler = db.urunler.ToList();          //select * from urunler
                model.sepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
                Response.Redirect(Request.RawUrl);
            }
            else if (btnLogout != null)
            {
                Session["loginDurum"] = 0;
                model.urunler = db.urunler.ToList();
                model.sepet = db.sepet.Where(x => x.kullaniciID == null).ToList();
                Response.Redirect(Request.RawUrl);
                Response.Clear();
                Response.BufferOutput = true;
            }
            
            if (btnSil!=null)
            {

                model.kategoriler = db.kategoriler.ToList();
                model.seciliUrun = null;
                model.seciliUrun = kategoriUrun;
                if (Convert.ToInt32( Session["loginDurum"])==1)
                {
                    int btnSilint = Convert.ToInt32(btnSil);
                    int adet=(from m in db.sepet
                     where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == btnSilint
                              select m.urunAdet).DefaultIfEmpty(0).Sum();
                    if (adet>1)
                    {
                        var sepetGüncelle = (from m in db.sepet
                                             where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == btnSilint
                                             select m).FirstOrDefault();

                        sepetGüncelle.urunAdet--;
                        db.SaveChanges();
                    }
                    else
                    {
                        Sepet sepetvol1= (from m in db.sepet
                                          where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == btnSilint
                                          select m).FirstOrDefault();
                        db.sepet.Remove(sepetvol1);
                        db.SaveChanges();
                    }
                    
                    model.sepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
                    Response.Redirect(Request.RawUrl);
                }
                //ZİYATETÇİ
                else
                {
                    int btnSilint = Convert.ToInt32(btnSil);
                    int adet = (from m in db.sepet
                                where m.kullaniciID == null && m.urunID.urunID == btnSilint
                                select m.urunAdet).DefaultIfEmpty(0).Sum();
                    if (adet > 1)
                    {
                        var sepetGüncelle = (from m in db.sepet
                                             where m.kullaniciID == null && m.urunID.urunID == btnSilint
                                             select m).FirstOrDefault();

                        sepetGüncelle.urunAdet--;
                        db.SaveChanges();
                    }
                    else
                    {
                        Sepet sepetvol1 = (from m in db.sepet
                                           where m.kullaniciID == null && m.urunID.urunID == btnSilint
                                           select m).FirstOrDefault();
                        db.sepet.Remove(sepetvol1);
                        db.SaveChanges();
                    }

                    model.sepet = db.sepet.Where(x => x.kullaniciID == null).ToList();
                    Response.Redirect(Request.RawUrl);
                }
               

            }
            ViewData["hide"] = "none";
            if (addCart != null)
            {
                Sepet sepet = new Sepet();
                sepet.urunID = (from m in db.urunler
                                where m.urunID == urunid
                                select m).FirstOrDefault();
                sepet.urunAdet = 1;
                model.kategoriler = db.kategoriler.ToList();
                model.seciliUrun = kategoriUrun;
                model.seciliUrun.sepetAdet++;
                
                db.SaveChanges();
                //ZİYARETÇİYSE
                if (Convert.ToInt32(Session["loginDurum"]) != 1)
                {
                    Session["sepetAdetZiyaretci"] = (from m in db.sepet
                                                     where m.kullaniciID == null
                                                     select m.urunAdet).DefaultIfEmpty(0).Sum();




                    var eklendiMi = (from m in db.sepet
                                     where m.urunID.urunID == urunid && m.kullaniciID == null
                                     select m).FirstOrDefault();
                    if (eklendiMi == null)
                    {

                        try
                        {
                            sepet.kullaniciID = (from m in db.kullanici
                                                 where m == null
                                                 select m).FirstOrDefault();

                            db.sepet.Add(sepet);                           
                            db.SaveChanges();
                            model.sepet = db.sepet.Where(x => x.kullaniciID == null).ToList();
                            Session["sepetAdetZiyaretci"] = Convert.ToInt32(Session["sepetAdetZiyaretci"]) + 1;
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    }
                    else
                    {
                        var sepetGüncelle = (from m in db.sepet
                                             where m.kullaniciID == null && m.urunID.urunID == urunid
                                             select m).FirstOrDefault();
                        
                        sepetGüncelle.urunAdet++;
                        db.SaveChanges();
                        if (db.sepet.Where(x => x.kullaniciID == null) == null)
                        {
                            model.sepet = null;
                        }
                        else
                        {
                            model.sepet = db.sepet.Where(x => x.kullaniciID == null).ToList();
                        }

                        Session["sepetAdetZiyaretci"] = Convert.ToInt32(Session["sepetAdetZiyaretci"]) + 1;
                    }
                    Response.Redirect(Request.RawUrl);
                }
                //KULLANICIYSA
                else if (Convert.ToInt32(Session["loginDurum"]) == 1)
                {

                    Session["sepetAdetKullanici"] = (from m in db.sepet
                                                     where m.kullaniciID.kullaniciID == kullaniciID
                                                     select m.urunAdet).DefaultIfEmpty(0).Sum();




                    var eklendiMi = (from m in db.sepet
                                     where m.urunID.urunID == urunid && m.kullaniciID.kullaniciID == kullaniciID
                                     select m).FirstOrDefault();
                    if (eklendiMi == null)
                    {

                        try
                        {
                            sepet.kullaniciID = (from m in db.kullanici
                                                 where m.kullaniciID == kullaniciID
                                                 select m).FirstOrDefault();
                         
                            db.sepet.Add(sepet);
                            db.SaveChanges();
                            model.sepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
                            Session["sepetAdetKullanici"] = Convert.ToInt32(Session["sepetAdetKullanici"]) + 1;
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    }
                    else
                    {
                        var sepetGüncelle = (from m in db.sepet
                                             where m.kullaniciID.kullaniciID == kullaniciID && m.urunID.urunID == urunid
                                             select m).FirstOrDefault();
                       
                        sepetGüncelle.urunAdet++;
                        db.SaveChanges();
                        if (db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID) == null)
                        {
                            model.sepet = null;
                        }
                        else
                        {
                            model.sepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
                        }

                        Session["sepetAdetKullanici"] = Convert.ToInt32(Session["sepetAdetKullanici"]) + 1;
                    }
                    Response.Redirect(Request.RawUrl);
                }


            }


            if (addFavorites != null)
            {
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
                model.sepet = db.sepet.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
            }


            model.kategoriler = db.kategoriler.ToList();
            model.seciliUrun = null;
            model.seciliUrun = kategoriUrun;
            Session["sepetAdetKullanici"] = (from m in db.sepet
                                             where m.kullaniciID.kullaniciID == kullaniciID
                                             select m.urunAdet).DefaultIfEmpty(0).Sum();
            Session["sepetAdetZiyaretci"] = (from m in db.sepet
                                             where m.kullaniciID == null
                                             select m.urunAdet).DefaultIfEmpty(0).Sum();
            
            return View("ProductDetails",model);
        }

    }
}