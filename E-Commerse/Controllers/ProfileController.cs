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
            if (Convert.ToInt32(Session["loginDurum"]) != 1)
            {
                return RedirectToAction("Index", "Home");
            }
            DatabaseContext db = new DatabaseContext();
            HomePageModel model = new HomePageModel();
            int id = Convert.ToInt32(Session["kullaniciID"]);
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
            if (urunid!=null)
            {
                Favorites fav = db.favorites.Where(X => X.urunID.urunID == urunid).FirstOrDefault();
                if (fav!=null)
                {
                    db.favorites.Remove(fav);
                    db.SaveChanges();
                }
                
            }

                var favoriurun = (from m in db.favorites
                                  where m.kullaniciID.kullaniciID == id
                                  select m).ToList();
                model.favorites = favoriurun;

            
            
            model.urunler = db.urunler.ToList();          //select * from urunler
            model.kategoriler = db.kategoriler.ToList();  //select * from kategoriler
            model.satinAlmaGecmis = db.satinAlmaGecmis.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
            return View(model);
        }
        // POST: Profile
        [HttpPost]
        public ActionResult ProfilePage(string btnLogout,string btnSil)
        {
            DatabaseContext db = new DatabaseContext();
            HomePageModel model = new HomePageModel();
            var kullaniciID = Convert.ToInt32(Session["kullaniciID"]);
            model.satinAlmaGecmis = db.satinAlmaGecmis.Where(x => x.kullaniciID.kullaniciID == kullaniciID).ToList();
            if (btnLogout != null)
            {
                Session["loginDurum"] = 0;
                model.urunler = db.urunler.ToList();
                model.sepet = db.sepet.Where(x => x.kullaniciID == null).ToList();
                return RedirectToAction("Index", "Home");
            }

            if (btnSil != null)
            {
                model.kategoriler = db.kategoriler.ToList();
                model.urunler = db.urunler.ToList();
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
            Session["sepetAdetKullanici"] = (from m in db.sepet
                                             where m.kullaniciID.kullaniciID == kullaniciID
                                             select m.urunAdet).DefaultIfEmpty(0).Sum();
            Session["sepetAdetZiyaretci"] = (from m in db.sepet
                                             where m.kullaniciID == null
                                             select m.urunAdet).DefaultIfEmpty(0).Sum();
            model.kategoriler = db.kategoriler.ToList(); //select * from kategoriler
            
            model.favorites = db.favorites.ToList();  
            return View(model);
        }
        public ActionResult AccountSetting()
        {
            if (Convert.ToInt32(Session["loginDurum"]) != 1)
            {
                return RedirectToAction("Index", "Home");
            }
            DatabaseContext db = new DatabaseContext();
            HomePageModel model = new HomePageModel();
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
            if (TempData["errorRegister"] != null)
            {

                ViewData["errorRegister"] = TempData["errorRegister"];

            }
            if (TempData["errorRegisterEmpty"] != null)
            {

                ViewData["errorRegisterEmpty"] = TempData["errorRegisterEmpty"];

            }
            model.kullanici = db.kullanici.Where(x => x.kullaniciID == kullaniciID).FirstOrDefault();
            model.kategoriler = db.kategoriler.ToList();  //select * from kategoriler
            return View(model);
        }
        [HttpPost]
        public ActionResult AccountSetting(string btnUpdate,string btnLogout,string btnSil, string eposta, string sifre, string btnLogin, string firstname, string lastname, string email, string password, string address, string country, string btnRegister, int? isRegisterOk)
        {
            DatabaseContext db = new DatabaseContext();
            HomePageModel model = new HomePageModel();
            var kullaniciID = Convert.ToInt32(Session["kullaniciID"]);
            model.kategoriler = db.kategoriler.ToList();
            model.kullanici = db.kullanici.Where(x => x.kullaniciID == kullaniciID).FirstOrDefault();
            if (firstname == "" || lastname == "" || email == "" || password == "" || address == "" || country == "")
            {
                TempData["errorRegisterEmpty"] = 1;
                return RedirectToAction("AccountSetting");
            }
            if (btnUpdate!=null)
            {
                var kullanici = model.kullanici;
                kullanici.kullaniciAd = firstname;
                kullanici.kullaniciSoyAd = lastname;
                kullanici.email = email;
                kullanici.kullaniciSifre = password;
                kullanici.Adres = address;
                kullanici.Country = country;
                Session["kullaniciAd"] = firstname;
                Session["kullaniciSoyAd"] = lastname;
                Session["kullaniciemail"] = email;
                Session["kullaniciSifre"] = password;
                Session["kullaniciAdres"] = address;
                Session["kullaniciUlke"] = country;
                db.SaveChanges();
                return RedirectToAction("AccountSetting");
            }
            if (btnSil != null)
            {
                model.kategoriler = db.kategoriler.ToList();
                model.urunler = db.urunler.ToList();
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
                    model.urunler = db.urunler.ToList();          //select * from urunler
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
                    return RedirectToAction("AccountSetting");
                }
                catch (Exception)
                {
                    //KULLANICI BULUNAMADI.
                    model.urunler = db.urunler.ToList();
                    ViewData["alertLogin"] = "1";

                }

            }
            if (btnLogout != null)
            {
                Session["loginDurum"] = 0;
                model.urunler = db.urunler.ToList();
                model.sepet = db.sepet.Where(x => x.kullaniciID == null).ToList();
                return RedirectToAction("Index", "Home");
            }
            
            return View(model);
        }
    }
}